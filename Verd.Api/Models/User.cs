namespace Verd.Api.Models;

public class User
{
    public int Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Tier { get; set; } = "Green Thumb";

    /// <summary>Gardener | Admin — drives access to the admin console.</summary>
    public string Role { get; set; } = "Gardener";
    public DateTime MemberSince { get; set; } = DateTime.UtcNow;
    public bool WeatherAlertsEnabled { get; set; } = true;
    public ICollection<Plant> Plants { get; set; } = [];
}
