using SteamClone.Backend.Entities;

namespace SteamClone.Backend.Services;

public interface IUserService
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<User> CreateAsync(User user, string password);
    Task<User> UpdateAsync(User user);
    Task<User> UpdatePasswordAsync(User user, string newPassword);
    bool VerifyPassword(User user, string password);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<bool> DeleteAsync(int id);
}