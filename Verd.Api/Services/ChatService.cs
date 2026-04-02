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
        var client = httpFactory.CreateClient("Anthropic");

        var messages = (history ?? [])
            .Select(m => new { role = m.Role, content = m.Content })
            .Append(new { role = "user", content = message })
            .ToList<object>();

        var body = new
        {
            model = "claude-opus-4-6",
            max_tokens = 1024,
            system = SystemPrompt,
            messages
        };

        var response = await client.PostAsJsonAsync("messages", body);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AnthropicResponse>();
        return result!.Content[0].Text;
    }
}

file record AnthropicResponse(
    [property: JsonPropertyName("content")] List<AnthropicContent> Content
);

file record AnthropicContent(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("text")] string Text
);
