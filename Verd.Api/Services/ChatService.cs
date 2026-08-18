using System.Text;
using System.Text.Json.Serialization;
using Verd.Api.DTOs.Chat;
using Verd.Api.Models;

namespace Verd.Api.Services;

public class ChatService(IHttpClientFactory httpFactory)
{
    private const string SystemPrompt = """
        You are Verd, a knowledgeable botanical AI specialist helping users care for their plants.
        Be concise, warm, and practical. Give actionable advice about watering, light, soil, pests,
        diseases, and general plant health. Keep responses focused and avoid unnecessary filler.

        Stay strictly on the topic of plants, gardening, and plant care. If the user asks about
        anything unrelated (for example recipes, coding, general trivia, or other off-topic
        requests), politely decline and steer the conversation back to their plants. Do not answer
        off-topic questions even if asked directly. Always respond in the same language the user
        writes in. Do not use Markdown tables: the chat panel is narrow. Present plant status or
        comparisons as short headings and bullet lists instead.
        """;

    public async Task<string> SendAsync(
        string message,
        IEnumerable<ChatMessageDto>? history,
        IEnumerable<Plant>? plants = null)
    {
        var client = httpFactory.CreateClient("Groq");

        var messages = new List<object>
        {
            new { role = "system", content = SystemPrompt + BuildPlantContext(plants) }
        };

        foreach (var m in history ?? [])
            messages.Add(new { role = m.Role, content = m.Content });

        messages.Add(new { role = "user", content = message });

        var body = new
        {
            // Groq retired Llama 3.3 70B for developer/free projects on
            // 2026-08-16. GPT-OSS 20B is its current fast production model.
            model = "openai/gpt-oss-20b",
            messages
        };

        var response = await client.PostAsJsonAsync("chat/completions", body);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GroqResponse>();
        return result!.Choices[0].Message.Content;
    }

    private static string BuildPlantContext(IEnumerable<Plant>? plants)
    {
        var list = plants?.ToList() ?? [];
        if (list.Count == 0)
            return "\n\nThe user has no plants in their garden yet. " +
                   "If they ask about \"my plants\", let them know their garden is empty and offer to help them add one.";

        var sb = new StringBuilder();
        sb.Append("\n\nHere is the user's current garden. When they refer to \"my plants\" or ask about a specific plant, use this list:\n");
        foreach (var p in list)
        {
            sb.Append($"- {p.Name}");
            if (!string.IsNullOrWhiteSpace(p.Category)) sb.Append($" ({p.Category})");
            sb.Append($": status {p.Status}, water level {p.WateringLevel}%, watering {p.WateringFrequency}, sunlight {p.Sunlight}, last watered {p.LastWatered}");
            if (!string.IsNullOrWhiteSpace(p.Notes)) sb.Append($", notes: {p.Notes}");
            sb.Append('\n');
        }
        sb.Append("If the user asks about a plant that is not in this list, you may still give general advice.");
        return sb.ToString();
    }
}

file record GroqResponse(
    [property: JsonPropertyName("choices")] List<GroqChoice> Choices
);

file record GroqChoice(
    [property: JsonPropertyName("message")] GroqMessage Message
);

file record GroqMessage(
    [property: JsonPropertyName("content")] string Content
);
