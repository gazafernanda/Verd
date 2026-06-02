using System.Text.Json;
using System.Text.Json.Serialization;
using Verd.Api.DTOs.Recommendations;
using Verd.Api.Models;

namespace Verd.Api.Services;

public class RecommendationAiService(IHttpClientFactory factory, WeatherService weatherService)
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public Task<RecommendationDto?> GenerateAsync(Plant plant, string userLocation) =>
        GenerateForGardenAsync([plant], userLocation);

    public async Task<RecommendationDto?> GenerateForGardenAsync(IEnumerable<Plant> plants, string userLocation)
    {
        var plantList = plants.ToList();

        string weatherContext = "Weather data unavailable.";
        var weather = await weatherService.GetForLocationAsync(userLocation);
        if (weather is not null)
        {
            weatherContext = $"Temperature: {weather.Temp}°C, Humidity: {weather.Humidity}%, " +
                             $"UV Index: {weather.UvIndex}, Wind: {weather.WindSpeed} km/h, " +
                             $"Conditions: {weather.Condition}";
        }

        var plantSummary = string.Join("\n", plantList.Select(p =>
            $"- {p.Name} ({p.Category}): watering {p.WateringFrequency}, sunlight {p.Sunlight}, status {p.Status}" +
            (string.IsNullOrWhiteSpace(p.Notes) ? "" : $", notes: {p.Notes}")));

        var jsonTemplate = """
            {
              "priorityActions": [
                { "id": "string", "title": "string", "description": "string", "priority": "IMMEDIATE|RECOMMENDED|OPTIONAL", "type": "water|mist|shade|fertilize|prune" }
              ],
              "insight": {
                "headline": "A single insightful botanical fact relevant to this garden and current weather",
                "detail": "2-3 sentences with specific actionable advice for these plants right now"
              }
            }
            """;

        var prompt = $"""
            You are a plant care specialist. Generate care recommendations for this garden given current weather conditions.

            Garden ({plantList.Count} plant{(plantList.Count == 1 ? "" : "s")}):
            {plantSummary}

            Current Weather at {userLocation}:
            {weatherContext}

            Return ONLY valid JSON (no markdown, no extra text) in this exact format:
            {jsonTemplate}

            Generate 3-5 priority actions specific to these plants and current weather. Prioritize any plants with status NEEDS WATER or NEEDS MISTING. Be practical and concise.
            """;

        var client = factory.CreateClient("Groq");
        var body = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new[] { new { role = "user", content = prompt } },
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
