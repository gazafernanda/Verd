namespace Verd.Api.DTOs.Users;

public record UpdateSettingsDto(
    string? DisplayName,
    string? Location,
    string? AvatarUrl,
    bool? WeatherAlertsEnabled
);
