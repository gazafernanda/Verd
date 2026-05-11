using System.ComponentModel.DataAnnotations;

namespace Verd.Api.DTOs.Plants;

public record UpsertPlantDto(
    [Required] string Name,
    string Status = "HEALTHY",
    int WateringLevel = 50,
    string LastWatered = "Today",
    string Category = "Tropical Plants",
    string IconBg = "#f0f9f4",
    string WateringFrequency = "every-2-days",
    string Sunlight = "indirect",
    string Notes = "",
    string CareCategory = "MAINTENANCE",
    string CareTitle = "",
    string CareDescription = "",
    string CareImage = "",
    string CareBgType = "green"
);
