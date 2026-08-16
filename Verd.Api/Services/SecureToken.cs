using System.Security.Cryptography;
using System.Text;

namespace Verd.Api.Services;

/// <summary>
/// Single-use, URL-safe tokens for the email verification and password reset links.
///
/// Only the SHA-256 hash is stored. A leaked database therefore can't be turned
/// into working links, and lookups still work because the hash is deterministic.
/// </summary>
public static class SecureToken
{
    /// <summary>Creates a token to email out, paired with the hash to store.</summary>
    public static (string Token, string Hash) Create()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        var token = Base64UrlEncode(bytes);
        return (token, Hash(token));
    }

    public static string Hash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token))).ToLowerInvariant();

    /// <summary>
    /// Compares in constant time so response timing can't be used to recover a
    /// valid token one character at a time.
    /// </summary>
    public static bool Matches(string token, string? storedHash)
    {
        if (string.IsNullOrEmpty(storedHash)) return false;
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(Hash(token)),
            Encoding.UTF8.GetBytes(storedHash));
    }

    private static string Base64UrlEncode(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
