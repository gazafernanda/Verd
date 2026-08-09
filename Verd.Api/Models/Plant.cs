namespace Verd.Api.Models;

public class Plant
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "HEALTHY"; // HEALTHY | NEEDS WATER | NEEDS MISTING
    public int WateringLevel { get; set; }
    public string LastWatered { get; set; } = string.Empty;

    /// <summary>
    /// When the plant was last watered, used to decay <see cref="WateringLevel"/>
    /// over time. Null for plants created before time tracking existed.
    /// </summary>
    public DateTime? LastWateredAt { get; set; }
    public string Category { get; set; } = string.Empty;
    public string IconBg { get; set; } = "#f0f9f4";

    // Care preferences
    public string WateringFrequency { get; set; } = "every-2-days"; // daily | every-2-days | weekly | biweekly | monthly
    public string Sunlight { get; set; } = "indirect"; // full-sun | partial | indirect | low
    public string Notes { get; set; } = string.Empty;

    // Care card
    public string CareCategory { get; set; } = string.Empty;
    public string CareTitle { get; set; } = string.Empty;
    public string CareDescription { get; set; } = string.Empty;
    public string CareImage { get; set; } = string.Empty;
    public string CareBgType { get; set; } = "green"; // blue | yellow | green

    public User User { get; set; } = null!;
    public ICollection<PlantLog> Logs { get; set; } = [];
}
