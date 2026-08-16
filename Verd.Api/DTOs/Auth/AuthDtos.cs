using System.ComponentModel.DataAnnotations;

namespace Verd.Api.DTOs.Auth;

/// <summary>The ID token issued by Google Identity Services in the browser.</summary>
public record GoogleSignInDto([Required] string Credential);

public record VerifyEmailDto([Required] string Token);

public record ResendVerificationDto([Required, EmailAddress] string Email);

public record ForgotPasswordDto([Required, EmailAddress] string Email);

public record ResetPasswordDto(
    [Required] string Token,
    [Required, MinLength(8)] string Password,
    [Required] string ConfirmPassword
);

/// <summary>
/// Returned when registration succeeds but the account still has to be verified.
/// Deliberately carries no token — an unverified account gets no session.
/// </summary>
public record PendingVerificationDto(
    string Email,
    string Message,
    bool EmailDelivered
);
