namespace Verd.Api.DTOs.Auth;

public record AuthResponseDto(
    string Token,
    string DisplayName,
    string Email,
    string Location,
    string Tier,
    string Role,
    bool IsEmailVerified,
    string AvatarUrl,
    string AuthProvider
);
