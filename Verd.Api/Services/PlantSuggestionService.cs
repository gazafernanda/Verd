using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Verd.Api.DTOs.Plants;

namespace Verd.Api.Services;

/// <summary>
/// Works out sensible care defaults from nothing but a plant name, so the user
/// doesn't have to know a Monstera's watering cycle to add one.
///
/// The model is asked for the exact enum values the Add Plant form uses, but its
/// output is still treated as untrusted: everything is normalised against the
/// allowed sets before it leaves this service, so a hallucinated
/// "twice-weekly" can never reach the UI and leave a chip unselectable.
/// </summary>
public partial class PlantSuggestionService(IHttpClientFactory factory, ILogger<PlantSuggestionService> log)
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    /// <summary>Must stay in step with the category chips in AddPlantView.</summary>
    private static readonly string[] Categories =
    [
        "Indoor Plants", "Outdoor Plants", "Succulents", "Herbs",
        "Vegetables", "Flowers", "Trees & Shrubs",
    ];

    private static readonly string[] WateringFrequencies =
        ["daily", "every-2-days", "weekly", "biweekly", "monthly"];

    private static readonly string[] SunlightLevels =
        ["full-sun", "partial", "indirect", "low"];

    public async Task<PlantSuggestionDto> SuggestAsync(string rawName, string? language)
    {
        var name = rawName.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return Unknown(name, isValid: false);

        var isIndonesian = language is not null &&
            language.StartsWith("id", StringComparison.OrdinalIgnoreCase);

        var languageInstruction = isIndonesian
            ? "Write the \"notes\" field in natural Indonesian (Bahasa Indonesia). Keep every other field exactly as specified in English."
            : "Write the \"notes\" field in English.";

        // $$ raw string: interpolation is {{expr}}, so the JSON braces below can
        // stay as literal single braces exactly as the model should see them.
        var prompt = $$"""
            A user is adding a plant to their garden app and typed: "{{name}}"

            Decide whether that names a real plant, plant species, or plant variety.
            If it does, fill in the care defaults a knowledgeable gardener would pick.

            Return ONLY valid JSON in this exact shape:
            {
              "isValid": true,
              "commonName": "The recognised common name, with typos corrected",
              "scientificName": "Genus species",
              "category": "one of: {{string.Join(" | ", Categories)}}",
              "wateringFrequency": "one of: {{string.Join(" | ", WateringFrequencies)}}",
              "sunlight": "one of: {{string.Join(" | ", SunlightLevels)}}",
              "notes": "One or two sentences of practical care advice specific to this plant."
            }

            Rules:
            - "isValid" must be false if "{{name}}" is not a plant (a person, object, brand, or nonsense). When false, still return the other fields as empty strings.
            - "category", "wateringFrequency" and "sunlight" MUST be copied verbatim from the lists above. Never invent a value.
            - Correct obvious misspellings in "commonName" (for example "monstra" is "Monstera", "lidah buaya" is "Aloe Vera").
            - If the plant is known by an Indonesian name, still return the standard common name.
            - Any temperature in "notes" MUST be in degrees Celsius, written like "18-24°C". Never use Fahrenheit — this app displays Celsius only.
            - {{languageInstruction}}
            """;

        var client = factory.CreateClient("Groq");
        var body = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new object[]
            {
                new { role = "system", content = "You are a botanist who returns strictly valid JSON and never adds commentary." },
                new { role = "user", content = prompt },
            },
            // Low but non-zero: identification should be near-deterministic, while
            // the free-text note still reads like prose rather than a template.
            temperature = 0.2,
            response_format = new { type = "json_object" },
        };

        try
        {
            var response = await client.PostAsJsonAsync("chat/completions", body);
            response.EnsureSuccessStatusCode();

            var groq = await response.Content.ReadFromJsonAsync<GroqResponse>(JsonOpts);
            var json = groq?.Choices?[0].Message.Content;
            if (string.IsNullOrWhiteSpace(json)) return Unknown(name, isValid: true);

            var raw = JsonSerializer.Deserialize<RawSuggestion>(json, JsonOpts);
            if (raw is null) return Unknown(name, isValid: true);

            if (!raw.IsValid)
                return new PlantSuggestionDto(
                    IsValid: false, CommonName: "", ScientificName: "", Category: "",
                    WateringFrequency: "", Sunlight: "", Notes: "", FromAi: true);

            return new PlantSuggestionDto(
                IsValid: true,
                CommonName: string.IsNullOrWhiteSpace(raw.CommonName) ? name : raw.CommonName.Trim(),
                ScientificName: raw.ScientificName?.Trim() ?? "",
                Category: Match(raw.Category, Categories) ?? "Indoor Plants",
                WateringFrequency: Match(raw.WateringFrequency, WateringFrequencies) ?? "weekly",
                Sunlight: Match(raw.Sunlight, SunlightLevels) ?? "indirect",
                Notes: ToCelsius(raw.Notes?.Trim() ?? ""),
                FromAi: true);
        }
        catch (Exception ex)
        {
            // Groq being down must never block adding a plant — the user just
            // fills the form in by hand, exactly as before this feature existed.
            log.LogWarning(ex, "Plant suggestion failed for '{Name}'; falling back to manual entry.", name);
            return Unknown(name, isValid: true);
        }
    }

    /// <summary>
    /// Rewrites any Fahrenheit the model slipped into the note as Celsius.
    ///
    /// The prompt asks for Celsius, but a prompt is a request rather than a
    /// constraint, and this text is stored on the plant and shown to the user —
    /// so the unit is enforced here, in the data layer, like every other reading.
    /// Handles both "70°F" and ranges like "65-75°F", where only the last number
    /// carries the unit.
    /// </summary>
    private static string ToCelsius(string notes)
    {
        if (string.IsNullOrWhiteSpace(notes)) return notes;

        return FahrenheitPattern().Replace(notes, match =>
        {
            var low = match.Groups["low"].Value;
            var high = match.Groups["high"].Value;

            if (!double.TryParse(high, NumberStyles.Any, CultureInfo.InvariantCulture, out var highF))
                return match.Value;

            var highC = Math.Round((highF - 32) * 5 / 9);

            if (string.IsNullOrEmpty(low) ||
                !double.TryParse(low, NumberStyles.Any, CultureInfo.InvariantCulture, out var lowF))
                return $"{highC:0}°C";

            var lowC = Math.Round((lowF - 32) * 5 / 9);
            return $"{lowC:0}-{highC:0}°C";
        });
    }

    /// <summary>
    /// Maps a model-supplied value onto the allowed set: exact match first, then
    /// case- and punctuation-insensitive, so "Full Sun" still resolves to "full-sun".
    /// Returns null when nothing matches, letting the caller pick the default.
    /// </summary>
    private static string? Match(string? value, string[] allowed)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        var exact = allowed.FirstOrDefault(a => a == value);
        if (exact is not null) return exact;

        var normalised = Normalise(value);
        return allowed.FirstOrDefault(a => Normalise(a) == normalised);
    }

    private static string Normalise(string value) =>
        new(value.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());

    /// <summary>
    /// The "we couldn't tell" result. <paramref name="isValid"/> is true when the
    /// AI simply wasn't reachable — refusing the plant because our own service
    /// failed would be the wrong call.
    /// </summary>
    private static PlantSuggestionDto Unknown(string name, bool isValid) => new(
        IsValid: isValid,
        CommonName: name,
        ScientificName: "",
        Category: "",
        WateringFrequency: "",
        Sunlight: "",
        Notes: "",
        FromAi: false);

    /// <summary>Matches "70°F", "70 F", and ranges such as "65-75°F".</summary>
    [GeneratedRegex(@"(?:(?<low>\d{1,3})\s*(?:°\s*F\b|\s*F\b)?\s*(?:-|–|to)\s*)?(?<high>\d{1,3})\s*°?\s*F\b",
        RegexOptions.IgnoreCase)]
    private static partial Regex FahrenheitPattern();
}

file record RawSuggestion(
    bool IsValid,
    string? CommonName,
    string? ScientificName,
    string? Category,
    string? WateringFrequency,
    string? Sunlight,
    string? Notes
);

file record GroqResponse(
    [property: JsonPropertyName("choices")] List<GroqChoice>? Choices
);
file record GroqChoice(
    [property: JsonPropertyName("message")] GroqMessage Message
);
file record GroqMessage(
    [property: JsonPropertyName("content")] string Content
);
