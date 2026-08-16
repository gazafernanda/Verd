namespace Verd.Api.DTOs.Plants;

/// <summary>
/// AI-proposed defaults for a plant the user is about to add.
///
/// Every field is normalised to a value the Add Plant form can actually select —
/// the model is asked for the app's enum values, and anything it invents is
/// mapped back or replaced with a default before it reaches the client.
/// </summary>
public record PlantSuggestionDto(
    /// <summary>False when the name isn't a real plant; the rest is then meaningless.</summary>
    bool IsValid,

    /// <summary>The recognised common name, spelling-corrected (e.g. "monstra" → "Monstera").</summary>
    string CommonName,

    string ScientificName,

    /// <summary>One of the form's categories, or a sensible free-text one.</summary>
    string Category,

    /// <summary>daily | every-2-days | weekly | biweekly | monthly</summary>
    string WateringFrequency,

    /// <summary>full-sun | partial | indirect | low</summary>
    string Sunlight,

    /// <summary>A short care note in the user's language, ready to drop into the form.</summary>
    string Notes,

    /// <summary>Whether the suggestion actually came from the model or is a fallback.</summary>
    bool FromAi
);

public record SuggestPlantDto(string Name, string? Language = null);
