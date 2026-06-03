namespace Verd.Api.DTOs.Recommendations;

public record GeneratePlantRecommendationDto(int PlantId);

public record RecommendationDto(
    IEnumerable<PriorityActionDto> PriorityActions,
    BotanicalInsightDto Insight,
    IEnumerable<DayPlanDto>? WeeklyPlan = null
);

public record DayPlanDto(
    string Day,       // TODAY | MON | TUE ...
    string Date,      // e.g. "Jun 5"
    string Weather,   // short summary e.g. "29°/22°C, sunny"
    string Action,    // the care action / note for that day
    string Type       // water | mist | shade | fertilize | prune | none
);

public record PriorityActionDto(
    string Id,
    string Title,
    string Description,
    string Priority,   // IMMEDIATE | RECOMMENDED | OPTIONAL
    string Type        // water | mist | shade | fertilize | prune
);

public record BotanicalInsightDto(
    string Headline,
    string Detail
);
