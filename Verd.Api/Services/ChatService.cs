using System.Text.Json.Serialization;
using Verd.Api.DTOs.Chat;

namespace Verd.Api.Services;

public class ChatService(IHttpClientFactory httpFactory)
{
    private const string SystemPrompt = """
        You are Verd, a knowledgeable botanical AI specialist helping users care for their plants.
        Be concise, warm, and practical. Give actionable advice about watering, light, soil, pests,
        diseases, and general plant health. Keep responses focused and avoid unnecessary filler.
        """;

    public async Task<string> SendAsync(string message, IEnumerable<ChatMessageDto>? history)
    {
        var client = httpFactory.CreateClient("Groq");

        var messages = new List<object>
        {
            new { role = "system", content = SystemPrompt }
        };

        foreach (var m in history ?? [])
            messages.Add(new { role = m.Role, content = m.Content });

        messages.Add(new { role = "user", content = message });

        var body = new
        {
            model = "llama-3.3-70b-versatile",
            messages
        };

        var response = await client.PostAsJsonAsync("chat/completions", body);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GroqResponse>();
        return result!.Choices[0].Message.Content;
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
