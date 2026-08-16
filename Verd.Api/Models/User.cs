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

    // ── Email verification ────────────────────────────────────────────────────
    /// <summary>
    /// Manual sign-ups start unverified and are held out of the core features
    /// until they click the emailed link. Google sign-ups are trusted straight
    /// away because Google has already proven the address.
    /// </summary>
    public bool IsEmailVerified { get; set; }

    /// <summary>Hash of the emailed verification token — never the token itself.</summary>
    public string? EmailVerificationTokenHash { get; set; }
    public DateTime? EmailVerificationExpiresAt { get; set; }

    /// <summary>Drives the resend cooldown so the mailer can't be used as a spam relay.</summary>
    public DateTime? VerificationEmailSentAt { get; set; }

    // ── Password reset ────────────────────────────────────────────────────────
    /// <summary>Hash of the emailed reset token. Cleared on use, so a link works once.</summary>
    public string? PasswordResetTokenHash { get; set; }
    public DateTime? PasswordResetExpiresAt { get; set; }

    // ── Federated identity ────────────────────────────────────────────────────
    /// <summary>Google's stable subject id, set once an account has been linked.</summary>
    public string? GoogleId { get; set; }

    /// <summary>local | google | local+google — recorded for support and auditing.</summary>
    public string AuthProvider { get; set; } = "local";

    public ICollection<Plant> Plants { get; set; } = [];
    public ICollection<ChatMessage> ChatMessages { get; set; } = [];
}
