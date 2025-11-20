namespace SteamClone.Backend.Entities;

/// <summary>
/// Represents user roles in the system with different permission levels
/// </summary>
public enum UserRole
{
    /// <summary>Regular customer who can browse, purchase, and review games</summary>
    Player,
    /// <summary>Game publisher who can add and manage their own games</summary>
    Publisher,
    /// <summary>System administrator with full access to all features</summary>
    Admin
}

/// <summary>
/// Represents a user account in the system
/// </summary>
public class User
{
    /// <summary>Unique identifier for the user</summary>
    public int Id { get; set; }

    /// <summary>Username for display and login purposes</summary>
    public required string Username { get; set; }

    /// <summary>Email address for authentication and communication</summary>
    public required string Email { get; set; }

    /// <summary>BCrypt hashed password for secure authentication</summary>
    public string? PasswordHash { get; set; }

    /// <summary>User's role determining their permissions in the system</summary>
    public UserRole Role { get; set; }

    /// <summary>Timestamp when the account was created</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Timestamp of the last profile update (null if never updated)</summary>
    public DateTime? UpdatedAt { get; set; }
}