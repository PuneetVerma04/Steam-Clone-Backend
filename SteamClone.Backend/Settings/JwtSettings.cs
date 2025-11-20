namespace SteamClone.Backend.Settings;

/// <summary>
/// Configuration settings for JWT (JSON Web Token) authentication
/// These values are loaded from appsettings.json
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Secret key used to sign and verify JWT tokens
    /// Should be a strong, random string stored securely
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Token issuer - identifies who created and signed the token
    /// Typically the application's domain or name
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Token audience - identifies who the token is intended for
    /// Typically the client application or API consumers
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time in minutes
    /// After this duration, the token becomes invalid and requires re-authentication
    /// </summary>
    public int ExpiryMinutes { get; set; }
}