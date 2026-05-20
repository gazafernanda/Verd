namespace Verd.Api.DTOs.Recommendations;

public record GeneratePlantRecommendationDto(int PlantId);

public record RecommendationDto(
    IEnumerable<PriorityActionDto> PriorityActions,
    BotanicalInsightDto Insight
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
