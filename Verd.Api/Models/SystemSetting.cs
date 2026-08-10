namespace Verd.Api.Models;

/// <summary>
/// Key/value store for the settings an admin can change at runtime, so tuning
/// the care rules does not require a redeploy.
/// </summary>
public class SystemSetting
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Defaults mirror the thresholds the care rules ship with.</summary>
    public static readonly Dictionary<string, string> Defaults = new()
    {
        ["uv.highThreshold"] = "6",
        ["weather.hotThresholdC"] = "30",
        ["weather.dryHumidityThreshold"] = "35",
        ["ai.enabled"] = "true",
        ["registration.open"] = "true",
        ["app.announcement"] = "",
    };
}
