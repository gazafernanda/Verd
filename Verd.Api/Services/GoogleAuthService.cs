using Google.Apis.Auth;

namespace Verd.Api.Services;

/// <summary>The trusted claims lifted from a validated Google ID token.</summary>
public record GoogleIdentity(
    string Subject,
    string Email,
    bool EmailVerified,
    string DisplayName,
    string PictureUrl
);

/// <summary>
/// Validates the ID token produced by Google Identity Services in the browser.
/// The signature is checked against Google's published keys, and the audience is
/// pinned to our own client id so a token minted for another site can't be replayed here.
/// </summary>
public class GoogleAuthService(IConfiguration config, ILogger<GoogleAuthService> log)
{
    public string? ClientId =>
        Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") is { Length: > 0 } fromEnv
            ? fromEnv
            : config["Google:ClientId"] is { Length: > 0 } fromConfig
                ? fromConfig
                : null;

    /// <summary>False when no client id is set, in which case sign-in must be refused.</summary>
    public bool IsConfigured => !string.IsNullOrWhiteSpace(ClientId);

    /// <summary>Returns null when the token is missing, expired, forged, or not ours.</summary>
    public async Task<GoogleIdentity?> ValidateAsync(string idToken)
    {
        if (!IsConfigured)
        {
            log.LogWarning("Google sign-in attempted but GOOGLE_CLIENT_ID is not configured.");
            return null;
        }

        if (string.IsNullOrWhiteSpace(idToken)) return null;

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = [ClientId!],
                });

            // Google can return an unverified address on some workspace configs.
            // Treating that as proof of ownership would let anyone claim an email.
            if (!payload.EmailVerified || string.IsNullOrWhiteSpace(payload.Email))
            {
                log.LogWarning("Google token for subject {Sub} has no verified email.", payload.Subject);
                return null;
            }

            return new GoogleIdentity(
                Subject: payload.Subject,
                Email: payload.Email.Trim().ToLowerInvariant(),
                EmailVerified: payload.EmailVerified,
                DisplayName: string.IsNullOrWhiteSpace(payload.Name) ? payload.Email : payload.Name,
                PictureUrl: payload.Picture ?? string.Empty
            );
        }
        catch (InvalidJwtException ex)
        {
            log.LogWarning(ex, "Rejected an invalid Google ID token.");
            return null;
        }
    }
}
