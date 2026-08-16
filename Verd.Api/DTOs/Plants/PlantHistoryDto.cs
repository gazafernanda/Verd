namespace Verd.Api.DTOs.Plants;

/// <summary>One row of the plant history list — every plant the user ever registered.</summary>
public record PlantHistoryDto(
    int Id,
    string Name,
    string Category,
    string IconBg,

    /// <summary>Start of the planting period.</summary>
    DateTime RegisteredAt,

    /// <summary>End of the planting period; null while the plant is still active.</summary>
    DateTime? EndedAt,

    /// <summary>ACTIVE | ENDED — whether the plant is still in the garden.</summary>
    string Status,

    /// <summary>The plant's care status; only meaningful while it is still active.</summary>
    string CareStatus,

    /// <summary>Whole days between registration and the end of the period (or today).</summary>
    int DurationDays,

    /// <summary>How many monitoring entries were recorded during the period.</summary>
    int LogCount
);

/// <summary>
/// The history list row plus the monitoring data recorded during that planting
/// period, for the detail view.
/// </summary>
public record PlantHistoryDetailDto(
    PlantHistoryDto Summary,
    string WateringFrequency,
    string Sunlight,
    string Notes,
    IEnumerable<PlantLogDto> Logs
);
