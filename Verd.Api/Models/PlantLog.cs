namespace Verd.Api.Models;

public class PlantLog
{
    public int Id { get; set; }
    public int PlantId { get; set; }
    public string Action { get; set; } = string.Empty; // watered | misted | fertilized | pruned | repotted
    public string Notes { get; set; } = string.Empty;
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;

    public Plant Plant { get; set; } = null!;
}
