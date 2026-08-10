using System.ComponentModel.DataAnnotations;

namespace Verd.Api.DTOs.Admin;

/// <summary>A user row in the admin console, with the counts an admin needs to judge activity.</summary>
public record AdminUserDto(
    int Id,
    string DisplayName,
    string Email,
    string Location,
    string Tier,
    string Role,
    DateTime MemberSince,
    bool WeatherAlertsEnabled,
    int PlantCount,
    int LogCount
);

public record UpdateAdminUserDto(
    [Required] string DisplayName,
    [Required, EmailAddress] string Email,
    string Location = "",
    string Tier = "Green Thumb",
    string Role = "Gardener",
    bool WeatherAlertsEnabled = true
);

public record ResetPasswordDto([Required, MinLength(8)] string NewPassword);

public record SystemSettingDto(string Key, string Value, DateTime UpdatedAt);

public record UpdateSystemSettingsDto(Dictionary<string, string> Settings);

/// <summary>Headline numbers for the admin dashboard.</summary>
public record AdminStatsDto(
    int TotalUsers,
    int TotalAdmins,
    int TotalPlants,
    int TotalLogs,
    int PlantsNeedingCare,
    int NewUsersThisWeek
);
