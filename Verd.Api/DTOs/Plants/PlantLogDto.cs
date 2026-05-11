using System.ComponentModel.DataAnnotations;

namespace Verd.Api.DTOs.Plants;

public record PlantLogDto(
    int Id,
    int PlantId,
    string Action,
    string Notes,
    DateTime LoggedAt
);

public record CreatePlantLogDto(
    [Required] string Action,
    string Notes = ""
);
