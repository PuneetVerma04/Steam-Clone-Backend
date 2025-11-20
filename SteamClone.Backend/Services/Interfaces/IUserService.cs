using SteamClone.Backend.Entities;

namespace SteamClone.Backend.Services;

public interface IUserService
{
    User? GetByEmail(string email);
    User? GetById(int id);
    User Create(User user, string password);
    User Update(User user);
    bool VerifyPassword(User user, string password);
    IEnumerable<User> GetAllUsers();
    bool Delete(int id);
}