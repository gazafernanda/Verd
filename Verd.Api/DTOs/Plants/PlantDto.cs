namespace Verd.Api.DTOs.Plants;

public record PlantDto(
    int Id,
    string Name,
    string Status,
    int WateringLevel,
    string LastWatered,
    string Category,
    string IconBg,
    string WateringFrequency,
    string Sunlight,
    string Notes,
    CareCardDto CareCard
);

public record CareCardDto(
    string Category,
    string Title,
    string Description,
    string Image,
    string BgType
);
