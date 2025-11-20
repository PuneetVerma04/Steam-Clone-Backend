using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SteamClone.Backend.Entities;
using SteamClone.Backend.Settings;


namespace SteamClone.Backend.Services;

/// <summary>
/// Service for generating JWT (JSON Web Tokens) for user authentication
/// </summary>
public class JwtService
{
    private readonly JwtSettings _jwtSettings;

    /// <summary>
    /// Initializes the JWT service with configuration settings
    /// </summary>
    public JwtService(IOptions<JwtSettings> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    /// <summary>
    /// Generates a JWT token for an authenticated user
    /// </summary>
    /// <param name="user">User entity to generate token for</param>
    /// <returns>Encoded JWT token string containing user claims</returns>
    public string GenerateToken(User user)
    {
        // Build claims collection with user information
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // User's numeric ID for authorization
            new Claim(ClaimTypes.Name, user.Username),                 // Username for display purposes
            new Claim(JwtRegisteredClaimNames.Email, user.Email),      // Email address
            new Claim(ClaimTypes.Role, user.Role.ToString()),          // User role (Player, Publisher, Admin)
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token identifier
        };

        // Create signing key from secret key in configuration
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Build JWT token with claims, expiration, and signature
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: creds
        );

        // Serialize token to string format
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}