namespace Verd.Api.Models;

public class Plant
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "HEALTHY"; // HEALTHY | NEEDS WATER | NEEDS MISTING
    public int WateringLevel { get; set; }
    public string LastWatered { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string IconBg { get; set; } = "#f0f9f4";

    // Care card
    public string CareCategory { get; set; } = string.Empty;
    public string CareTitle { get; set; } = string.Empty;
    public string CareDescription { get; set; } = string.Empty;
    public string CareImage { get; set; } = string.Empty;
    public string CareBgType { get; set; } = "green"; // blue | yellow | green

    public User User { get; set; } = null!;
}
