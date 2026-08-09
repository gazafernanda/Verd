using Verd.Api.Models;

namespace Verd.Api.Services;

/// <summary>
/// Derives a plant's current water level from how long ago it was last watered.
/// The level decays linearly across one watering cycle, so a plant left alone
/// eventually reports NEEDS MISTING and then NEEDS WATER on its own — without
/// the user having to move the slider by hand.
/// </summary>
public static class PlantCareService
{
    public const int NeedsWaterAtOrBelow = 25;
    public const int NeedsMistingAtOrBelow = 50;

    /// <summary>Days it takes a full tank (100%) to run dry for a given frequency.</summary>
    public static double CycleDays(string wateringFrequency) => wateringFrequency switch
    {
        "daily" => 1,
        "every-2-days" => 2,
        "weekly" => 7,
        "biweekly" => 14,
        "monthly" => 30,
        _ => 2,
    };

    /// <summary>
    /// Water level right now. Plants created before time tracking existed have no
    /// <see cref="Plant.LastWateredAt"/>, so their stored level is used as-is.
    /// </summary>
    public static int CurrentLevel(Plant plant, DateTime utcNow)
    {
        if (plant.LastWateredAt is not { } wateredAt) return plant.WateringLevel;

        var elapsedDays = (utcNow - wateredAt).TotalDays;
        if (elapsedDays <= 0) return 100;

        var remaining = 100 * (1 - elapsedDays / CycleDays(plant.WateringFrequency));
        return (int)Math.Round(Math.Clamp(remaining, 0, 100));
    }

    public static string StatusFor(int level) => level switch
    {
        <= NeedsWaterAtOrBelow => "NEEDS WATER",
        <= NeedsMistingAtOrBelow => "NEEDS MISTING",
        _ => "HEALTHY",
    };

    /// <summary>
    /// Timestamp that makes <see cref="CurrentLevel"/> return <paramref name="level"/>
    /// right now. Lets a manually chosen slider value decay from that point on
    /// instead of being frozen.
    /// </summary>
    public static DateTime BaselineFor(int level, string wateringFrequency, DateTime utcNow)
    {
        var clamped = Math.Clamp(level, 0, 100);
        var elapsedDays = CycleDays(wateringFrequency) * (1 - clamped / 100.0);
        return utcNow.AddDays(-elapsedDays);
    }

    /// <summary>Human-readable "last watered" text derived from the timestamp.</summary>
    public static string LastWateredLabel(Plant plant, DateTime utcNow)
    {
        if (plant.LastWateredAt is not { } wateredAt) return plant.LastWatered;

        var elapsed = utcNow - wateredAt;
        if (elapsed.TotalMinutes < 60) return "Just now";
        if (elapsed.TotalHours < 24)
        {
            var hours = (int)elapsed.TotalHours;
            return hours == 1 ? "1 hour ago" : $"{hours} hours ago";
        }

        var days = (int)elapsed.TotalDays;
        return days == 1 ? "Yesterday" : $"{days} days ago";
    }
}
