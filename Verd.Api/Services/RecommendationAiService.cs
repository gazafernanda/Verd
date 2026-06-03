using System.Text.Json;
using System.Text.Json.Serialization;
using Verd.Api.DTOs.Recommendations;
using Verd.Api.DTOs.Weather;
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
        string forecastContext = "7-day forecast unavailable.";
        WeatherDto? weather = null;
        try
        {
            weather = await weatherService.GetForLocationAsync(userLocation);
        }
        catch
        {
            // Weather is best-effort — fall back to a generic plan if it fails.
        }
        if (weather is not null)
        {
            weatherContext = $"Temperature: {weather.Temp}°C, Humidity: {weather.Humidity}%, " +
                             $"UV Index: {weather.UvIndex}, Wind: {weather.WindSpeed} km/h, " +
                             $"Conditions: {weather.Condition}";

            forecastContext = string.Join("\n", weather.Forecast.Select(f =>
                $"- {f.Day} ({f.Date}): high {f.TempHi}°C / low {f.TempLo}°C, {f.Icon}"));
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
              },
              "weeklyPlan": [
                { "day": "TODAY", "date": "Jun 5", "weather": "29°/22°C, sunny", "action": "Concise care task for this plant on this specific day, tied to that day's weather.", "type": "water|mist|shade|fertilize|prune|none" }
              ]
            }
            """;

        var prompt = $"""
            You are a plant care specialist. Generate care recommendations for this garden given the current weather and the 7-day forecast.

            Garden ({plantList.Count} plant{(plantList.Count == 1 ? "" : "s")}):
            {plantSummary}

            Current Weather at {userLocation}:
            {weatherContext}

            7-Day Forecast at {userLocation}:
            {forecastContext}

            Return ONLY valid JSON (no markdown, no extra text) in this exact format:
            {jsonTemplate}

            Requirements:
            - "priorityActions": 3-5 actions for right now, specific to these plants and current weather. Prioritize plants with status NEEDS WATER or NEEDS MISTING.
            - "weeklyPlan": EXACTLY one entry per day for all 7 forecast days, in order. Use the same "day" and "date" labels as the forecast above. For each day, give ONE concise care action tied to that day's predicted weather (e.g. skip watering before/on rainy days, water more on hot dry days, add shade on high-UV days). If no action is needed that day, use action like "No action needed — monitor" and type "none".
            - Be practical and concise.
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
