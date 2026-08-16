using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Verd.Api.Data;
using Verd.Api.DTOs.Auth;
using Verd.Api.Models;
using Verd.Api.Services;

namespace Verd.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public partial class AuthController(
    AppDbContext db,
    JwtService jwt,
    EmailService email,
    GoogleAuthService google,
    IConfiguration config,
    ILogger<AuthController> log) : ControllerBase
{
    private static readonly TimeSpan VerificationLifetime = TimeSpan.FromHours(24);
    private static readonly TimeSpan ResetLifetime = TimeSpan.FromHours(1);

    /// <summary>How long a user must wait between verification emails.</summary>
    private static readonly TimeSpan ResendCooldown = TimeSpan.FromSeconds(60);

    // ── Registration ──────────────────────────────────────────────────────────

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        var normalisedEmail = Normalise(dto.Email);

        if (await db.Users.AnyAsync(u => u.Email.ToLower() == normalisedEmail))
            return Conflict(new { message = "Email already in use." });

        if (PasswordProblem(dto.Password) is { } problem)
            return BadRequest(new { message = problem });

        var (token, hash) = SecureToken.Create();

        var user = new User
        {
            DisplayName = dto.DisplayName,
            Email = normalisedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Location = dto.Location,
            MemberSince = DateTime.UtcNow,
            AuthProvider = "local",
            // Manual sign-ups have proved nothing yet — the emailed link does that.
            IsEmailVerified = false,
            EmailVerificationTokenHash = hash,
            EmailVerificationExpiresAt = DateTime.UtcNow.Add(VerificationLifetime),
            VerificationEmailSentAt = DateTime.UtcNow,
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        await email.SendVerificationAsync(user.Email, user.DisplayName, VerificationLink(token));

        // A session is issued straight away so the user lands in the app and can
        // resend the email, but every core feature stays closed until they verify.
        return CreatedAtAction(nameof(Register), ToResponse(user));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var normalisedEmail = Normalise(dto.Email);
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalisedEmail);

        if (user is null || string.IsNullOrEmpty(user.PasswordHash)
            || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });

        return Ok(ToResponse(user));
    }

    // ── Google sign-in ────────────────────────────────────────────────────────

    /// <summary>
    /// Exposes the client id so the frontend can render the Google button without
    /// the value being baked into the bundle at build time.
    /// </summary>
    [HttpGet("google/config")]
    public ActionResult GoogleConfig() =>
        Ok(new { clientId = google.ClientId ?? "", enabled = google.IsConfigured });

    [HttpPost("google")]
    public async Task<ActionResult<AuthResponseDto>> GoogleSignIn(GoogleSignInDto dto)
    {
        if (!google.IsConfigured)
            return StatusCode(StatusCodes.Status503ServiceUnavailable,
                new { message = "Google sign-in is not configured on this server." });

        var identity = await google.ValidateAsync(dto.Credential);
        if (identity is null)
            return Unauthorized(new { message = "Google sign-in could not be verified." });

        // Already linked — straight through.
        var user = await db.Users.FirstOrDefaultAsync(u => u.GoogleId == identity.Subject);

        if (user is null)
        {
            // Same person, existing manual account: link the two rather than
            // creating a second account for an address that is already taken.
            user = await db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == identity.Email);

            if (user is not null)
            {
                user.GoogleId = identity.Subject;
                user.AuthProvider = string.IsNullOrEmpty(user.PasswordHash) ? "google" : "local+google";

                // Google has proven ownership of the address, so a pending
                // verification on the manual account is now satisfied.
                user.IsEmailVerified = true;
                user.EmailVerificationTokenHash = null;
                user.EmailVerificationExpiresAt = null;

                if (string.IsNullOrWhiteSpace(user.AvatarUrl) && !string.IsNullOrWhiteSpace(identity.PictureUrl))
                    user.AvatarUrl = identity.PictureUrl;

                await db.SaveChangesAsync();
                log.LogInformation("Linked Google identity to existing account {Email}.", user.Email);
            }
            else
            {
                // Brand new Google user — provisioned without a second verification
                // step, because Google already verified the address.
                user = new User
                {
                    DisplayName = identity.DisplayName,
                    Email = identity.Email,
                    PasswordHash = string.Empty,
                    Location = string.Empty,
                    AvatarUrl = identity.PictureUrl,
                    MemberSince = DateTime.UtcNow,
                    GoogleId = identity.Subject,
                    AuthProvider = "google",
                    IsEmailVerified = true,
                };
                db.Users.Add(user);
                await db.SaveChangesAsync();
                log.LogInformation("Provisioned new Google account {Email}.", user.Email);
            }
        }

        return Ok(ToResponse(user));
    }

    // ── Email verification ────────────────────────────────────────────────────

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailDto dto)
    {
        var hash = SecureToken.Hash(dto.Token);
        var user = await db.Users.FirstOrDefaultAsync(u => u.EmailVerificationTokenHash == hash);

        if (user is null)
            return BadRequest(new
            {
                message = "Tautan verifikasi tidak valid atau sudah digunakan. Silakan minta tautan baru.",
                code = "invalid_token",
            });

        if (user.EmailVerificationExpiresAt is null || user.EmailVerificationExpiresAt < DateTime.UtcNow)
        {
            // Leave the hash in place so a second click still reports "expired"
            // rather than the more confusing "invalid".
            return BadRequest(new
            {
                message = "Tautan verifikasi sudah kedaluwarsa. Silakan minta tautan baru.",
                code = "expired_token",
            });
        }

        user.IsEmailVerified = true;
        user.EmailVerificationTokenHash = null;
        user.EmailVerificationExpiresAt = null;
        await db.SaveChangesAsync();

        // A fresh token so the client immediately reflects the verified state.
        return Ok(ToResponse(user));
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification(ResendVerificationDto dto)
    {
        var normalisedEmail = Normalise(dto.Email);
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalisedEmail);

        // Same answer whether or not the address exists, so this endpoint can't be
        // used to enumerate accounts.
        var neutral = Ok(new
        {
            message = "Jika alamat tersebut membutuhkan verifikasi, kami telah mengirim tautan baru.",
        });

        if (user is null || user.IsEmailVerified) return neutral;

        if (user.VerificationEmailSentAt is { } sentAt)
        {
            var elapsed = DateTime.UtcNow - sentAt;
            if (elapsed < ResendCooldown)
            {
                var retryAfter = (int)Math.Ceiling((ResendCooldown - elapsed).TotalSeconds);
                Response.Headers.RetryAfter = retryAfter.ToString();
                return StatusCode(StatusCodes.Status429TooManyRequests, new
                {
                    message = $"Mohon tunggu {retryAfter} detik sebelum meminta email verifikasi lagi.",
                    retryAfter,
                    code = "cooldown",
                });
            }
        }

        var (token, hash) = SecureToken.Create();
        user.EmailVerificationTokenHash = hash;
        user.EmailVerificationExpiresAt = DateTime.UtcNow.Add(VerificationLifetime);
        user.VerificationEmailSentAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        await email.SendVerificationAsync(user.Email, user.DisplayName, VerificationLink(token));
        return neutral;
    }

    /// <summary>Lets a signed-in but unverified user trigger a resend without retyping their address.</summary>
    [Authorize]
    [HttpPost("resend-verification/me")]
    public async Task<IActionResult> ResendVerificationForCurrentUser()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!int.TryParse(raw, out var userId)) return Unauthorized();

        var user = await db.Users.FindAsync(userId);
        if (user is null) return Unauthorized();

        return await ResendVerification(new ResendVerificationDto(user.Email));
    }

    // ── Password reset ────────────────────────────────────────────────────────

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
    {
        var normalisedEmail = Normalise(dto.Email);
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalisedEmail);

        if (user is not null)
        {
            var (token, hash) = SecureToken.Create();
            user.PasswordResetTokenHash = hash;
            user.PasswordResetExpiresAt = DateTime.UtcNow.Add(ResetLifetime);
            await db.SaveChangesAsync();

            await email.SendPasswordResetAsync(user.Email, user.DisplayName, ResetLink(token));
        }
        else
        {
            log.LogInformation("Password reset requested for an address with no account.");
        }

        // Identical response either way — the caller must not be able to tell
        // whether the address is registered.
        return Ok(new
        {
            message = "Jika alamat tersebut terdaftar, kami telah mengirim tautan untuk mengatur ulang kata sandi.",
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        if (dto.Password != dto.ConfirmPassword)
            return BadRequest(new { message = "Konfirmasi kata sandi tidak cocok.", code = "mismatch" });

        if (PasswordProblem(dto.Password) is { } problem)
            return BadRequest(new { message = problem, code = "weak_password" });

        var hash = SecureToken.Hash(dto.Token);
        var user = await db.Users.FirstOrDefaultAsync(u => u.PasswordResetTokenHash == hash);

        if (user is null || user.PasswordResetExpiresAt is null || user.PasswordResetExpiresAt < DateTime.UtcNow)
            return BadRequest(new
            {
                message = "Tautan atur ulang tidak valid atau sudah kedaluwarsa. Silakan minta tautan baru.",
                code = "invalid_token",
            });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // Clearing the hash is what makes the link single-use.
        user.PasswordResetTokenHash = null;
        user.PasswordResetExpiresAt = null;

        // Receiving mail at the address proves ownership just as the signup link would.
        user.IsEmailVerified = true;
        user.EmailVerificationTokenHash = null;
        user.EmailVerificationExpiresAt = null;

        if (user.AuthProvider == "google") user.AuthProvider = "local+google";

        await db.SaveChangesAsync();

        return Ok(new { message = "Kata sandi berhasil diperbarui. Silakan masuk dengan kata sandi baru Anda." });
    }

    /// <summary>Reports whether a reset link is still usable, so the page can fail fast.</summary>
    [HttpGet("reset-password/valid")]
    public async Task<IActionResult> IsResetTokenValid([FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return Ok(new { valid = false });

        var hash = SecureToken.Hash(token);
        var expiry = await db.Users
            .Where(u => u.PasswordResetTokenHash == hash)
            .Select(u => u.PasswordResetExpiresAt)
            .FirstOrDefaultAsync();

        return Ok(new { valid = expiry is not null && expiry > DateTime.UtcNow });
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string Normalise(string email) => email.Trim().ToLowerInvariant();

    /// <summary>Returns a message describing why the password is too weak, or null if it's fine.</summary>
    private static string? PasswordProblem(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return "Kata sandi minimal 8 karakter.";
        if (!LetterPattern().IsMatch(password))
            return "Kata sandi harus mengandung setidaknya satu huruf.";
        if (!DigitPattern().IsMatch(password))
            return "Kata sandi harus mengandung setidaknya satu angka.";
        return null;
    }

    /// <summary>
    /// Where the emailed links point. The frontend uses hash routing under a
    /// "/Verd/" base, so the trailing "#/" is part of the address, not decoration.
    /// </summary>
    private string AppBaseUrl =>
        (Environment.GetEnvironmentVariable("APP_BASE_URL")
         ?? config["App:BaseUrl"]
         ?? "http://localhost:5173/Verd/").TrimEnd('/');

    private string VerificationLink(string token) =>
        $"{AppBaseUrl}/#/verify-email?token={Uri.EscapeDataString(token)}";

    private string ResetLink(string token) =>
        $"{AppBaseUrl}/#/reset-password?token={Uri.EscapeDataString(token)}";

    private AuthResponseDto ToResponse(User user) => new(
        Token: jwt.GenerateToken(user),
        DisplayName: user.DisplayName,
        Email: user.Email,
        Location: user.Location,
        Tier: user.Tier,
        Role: user.Role,
        IsEmailVerified: user.IsEmailVerified,
        AvatarUrl: user.AvatarUrl,
        AuthProvider: user.AuthProvider
    );

    [GeneratedRegex("[A-Za-z]")]
    private static partial Regex LetterPattern();

    [GeneratedRegex("[0-9]")]
    private static partial Regex DigitPattern();
}
