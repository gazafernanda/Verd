namespace Verd.Api.DTOs.Users;

public record UserProfileDto(
    int Id,
    string DisplayName,
    string Email,
    string Location,
    string Tier,
    string MemberSince,
    bool WeatherAlertsEnabled
);
