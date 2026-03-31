namespace Verd.Api.DTOs.Users;

public record UpdateSettingsDto(
    string? DisplayName,
    string? Location,
    bool? WeatherAlertsEnabled
);
