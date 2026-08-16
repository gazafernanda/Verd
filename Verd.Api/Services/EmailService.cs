using System.Net;
using System.Net.Mail;

namespace Verd.Api.Services;

/// <summary>
/// Sends the transactional mail the auth flows depend on.
///
/// When SMTP isn't configured — which is the normal case for local development —
/// the message is written to the log instead of being dropped silently, so the
/// verification and reset links are still reachable while working offline.
/// </summary>
public class EmailService(IConfiguration config, ILogger<EmailService> log)
{
    private string? Host => Env("SMTP_HOST", "Smtp:Host");
    private string? Username => Env("SMTP_USERNAME", "Smtp:Username");
    private string? Password => Env("SMTP_PASSWORD", "Smtp:Password");
    private string FromAddress => Env("SMTP_FROM", "Smtp:From") ?? "no-reply@verd.app";
    private string FromName => Env("SMTP_FROM_NAME", "Smtp:FromName") ?? "Verd";

    private int Port =>
        int.TryParse(Env("SMTP_PORT", "Smtp:Port"), out var p) ? p : 587;

    private bool UseSsl =>
        !bool.TryParse(Env("SMTP_USE_SSL", "Smtp:UseSsl"), out var s) || s;

    /// <summary>True when real mail can actually be delivered.</summary>
    public bool IsConfigured => !string.IsNullOrWhiteSpace(Host);

    private string? Env(string variable, string configKey) =>
        Environment.GetEnvironmentVariable(variable) is { Length: > 0 } value
            ? value
            : config[configKey] is { Length: > 0 } fromConfig
                ? fromConfig
                : null;

    public async Task SendAsync(string toEmail, string subject, string htmlBody, string textBody)
    {
        if (!IsConfigured)
        {
            // Deliberately logged in full: without a mail server there is no other
            // way for a developer to complete a verification or reset flow.
            log.LogWarning(
                "SMTP is not configured — email to {To} was not sent.\nSubject: {Subject}\n{Body}",
                toEmail, subject, textBody);
            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(FromAddress, FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true,
        };
        message.To.Add(toEmail);

        using var client = new SmtpClient(Host, Port) { EnableSsl = UseSsl };
        if (!string.IsNullOrWhiteSpace(Username))
            client.Credentials = new NetworkCredential(Username, Password);

        try
        {
            await client.SendMailAsync(message);
            log.LogInformation("Sent '{Subject}' to {To}.", subject, toEmail);
        }
        catch (Exception ex)
        {
            // A mail outage must not turn into a failed registration or a reset
            // response that reveals whether the address exists.
            log.LogError(ex, "Failed to send '{Subject}' to {To}.", subject, toEmail);
        }
    }

    public Task SendVerificationAsync(string toEmail, string displayName, string link)
    {
        const string subject = "Verifikasi alamat email Verd Anda";
        var text = $"""
            Halo {displayName},

            Terima kasih telah mendaftar di Verd. Klik tautan berikut untuk memverifikasi
            alamat email Anda:

            {link}

            Tautan ini berlaku selama 24 jam. Jika Anda tidak membuat akun ini, abaikan email ini.
            """;

        return SendAsync(toEmail, subject, Wrap(
            title: "Verifikasi email Anda",
            greeting: $"Halo {WebUtility.HtmlEncode(displayName)},",
            paragraph: "Terima kasih telah mendaftar di Verd. Klik tombol di bawah untuk memverifikasi alamat email Anda.",
            buttonLabel: "Verifikasi Email",
            link: link,
            footer: "Tautan ini berlaku selama 24 jam. Jika Anda tidak membuat akun ini, abaikan email ini."
        ), text);
    }

    public Task SendPasswordResetAsync(string toEmail, string displayName, string link)
    {
        const string subject = "Atur ulang kata sandi Verd Anda";
        var text = $"""
            Halo {displayName},

            Kami menerima permintaan untuk mengatur ulang kata sandi akun Verd Anda.
            Klik tautan berikut untuk membuat kata sandi baru:

            {link}

            Tautan ini berlaku selama 1 jam dan hanya dapat digunakan sekali.
            Jika Anda tidak meminta ini, abaikan email ini — kata sandi Anda tidak berubah.
            """;

        return SendAsync(toEmail, subject, Wrap(
            title: "Atur ulang kata sandi",
            greeting: $"Halo {WebUtility.HtmlEncode(displayName)},",
            paragraph: "Kami menerima permintaan untuk mengatur ulang kata sandi akun Verd Anda. Klik tombol di bawah untuk membuat kata sandi baru.",
            buttonLabel: "Atur Ulang Kata Sandi",
            link: link,
            footer: "Tautan ini berlaku selama 1 jam dan hanya dapat digunakan sekali. Jika Anda tidak meminta ini, abaikan email ini — kata sandi Anda tidak berubah."
        ), text);
    }

    private static string Wrap(
        string title, string greeting, string paragraph,
        string buttonLabel, string link, string footer)
    {
        var safeLink = WebUtility.HtmlEncode(link);
        return $"""
            <div style="font-family:-apple-system,Segoe UI,Roboto,sans-serif;max-width:520px;margin:0 auto;padding:32px 24px;color:#1f2933">
              <div style="font-size:20px;font-weight:700;color:#1a5641;margin-bottom:24px">Verd</div>
              <h1 style="font-size:22px;margin:0 0 16px">{WebUtility.HtmlEncode(title)}</h1>
              <p style="margin:0 0 12px">{greeting}</p>
              <p style="margin:0 0 24px;line-height:1.6">{WebUtility.HtmlEncode(paragraph)}</p>
              <a href="{safeLink}"
                 style="display:inline-block;background:#1a5641;color:#fff;text-decoration:none;
                        padding:12px 24px;border-radius:24px;font-weight:600">
                {WebUtility.HtmlEncode(buttonLabel)}
              </a>
              <p style="margin:24px 0 0;font-size:13px;color:#6b7280;line-height:1.6">
                {WebUtility.HtmlEncode(footer)}
              </p>
              <p style="margin:16px 0 0;font-size:12px;color:#9ca3af;word-break:break-all">{safeLink}</p>
            </div>
            """;
    }
}
