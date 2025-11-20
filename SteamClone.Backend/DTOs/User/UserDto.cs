namespace SteamClone.Backend.DTOs.User;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!; // e.g., "User", "Admin"
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } // Nullable to allow for users that haven't been updated
}