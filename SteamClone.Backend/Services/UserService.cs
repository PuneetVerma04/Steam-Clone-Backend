using SteamClone.Backend.Entities;
using AutoMapper;
using BCrypt.Net;

namespace SteamClone.Backend.Services;

public class UserService : IUserService
{
    private readonly BackendDbContext _dbContext;
    private readonly IMapper _mapper;

    public UserService(BackendDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public User? GetByEmail(string email)
    {
        return _dbContext.Users.FirstOrDefault(u => u.Email == email);
    }

    public User? GetById(int id)
    {
        return _dbContext.Users.Find(id);
    }

    public User Create(User user, string rawPassword)
    {
        user.CreatedAt = DateTime.UtcNow;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(rawPassword);

        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();

        return user;
    }

    public bool VerifyPassword(User user, string password)
    {
        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            return false;
        }
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _dbContext.Users.ToList();
    }

    public bool Delete(int id)
    {
        var user = _dbContext.Users.Find(id);
        if (user == null) return false;

        _dbContext.Users.Remove(user);
        _dbContext.SaveChanges();

        return true;
    }
}