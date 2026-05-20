using System.Text.Json;
using System.Text.Json.Serialization;
using Verd.Api.DTOs.Recommendations;
using Verd.Api.Models;

namespace Verd.Api.Services;

public class RecommendationAiService(IHttpClientFactory factory, WeatherService weatherService)
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public async Task<RecommendationDto?> GenerateAsync(Plant plant, string userLocation)
    {
        // Fetch real weather for context
        string weatherContext = "Weather data unavailable.";
        var weather = await weatherService.GetForLocationAsync(userLocation);
        if (weather is not null)
        {
            weatherContext = $"Temperature: {weather.Temp}°C, Humidity: {weather.Humidity}%, " +
                             $"UV Index: {weather.UvIndex}, Wind: {weather.WindSpeed} km/h, " +
                             $"Conditions: {weather.Condition}";
        }

        var jsonTemplate = """
            {
              "priorityActions": [
                { "id": "string", "title": "string", "description": "string", "priority": "IMMEDIATE|RECOMMENDED|OPTIONAL", "type": "water|mist|shade|fertilize|prune" }
              ],
              "insight": {
                "headline": "A single insightful botanical fact relevant to this plant and the current weather",
                "detail": "2-3 sentences with specific actionable advice for this plant right now"
              }
            }
            """;

        var prompt = $"""
            You are a plant care specialist. Generate specific care recommendations for the following plant given current weather conditions.

            Plant Details:
            - Name: {plant.Name}
            - Category: {plant.Category}
            - Watering Frequency: {plant.WateringFrequency}
            - Sunlight Needs: {plant.Sunlight}
            - Notes: {(string.IsNullOrWhiteSpace(plant.Notes) ? "None" : plant.Notes)}

            Current Weather at {userLocation}:
            {weatherContext}

            Return ONLY valid JSON (no markdown, no extra text) in this exact format:
            {jsonTemplate}

            Generate 3-4 priority actions that are highly specific to this plant and current weather. Be practical and concise.
            """;

        var client = factory.CreateClient("Groq");
        var body = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            temperature = 0.4,
            response_format = new { type = "json_object" }
        };

        try
        {
            var response = await client.PostAsJsonAsync("chat/completions", body);
            response.EnsureSuccessStatusCode();

            var groqResult = await response.Content.ReadFromJsonAsync<GroqResponse>(JsonOpts);
            var json = groqResult?.Choices?[0].Message.Content ?? "";

            return JsonSerializer.Deserialize<RecommendationDto>(json, JsonOpts);
        }
        catch
        {
            return null;
        }
    }
}

file record GroqResponse(
    [property: JsonPropertyName("choices")] List<GroqChoice>? Choices
);
file record GroqChoice(
    [property: JsonPropertyName("message")] GroqMessage Message
);
file record GroqMessage(
    [property: JsonPropertyName("content")] string Content
);
