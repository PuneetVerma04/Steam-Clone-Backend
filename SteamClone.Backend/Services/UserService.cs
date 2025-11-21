using SteamClone.Backend.Entities;
using AutoMapper;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace SteamClone.Backend.Services;

/// <summary>
/// Service for managing user accounts including authentication and CRUD operations
/// </summary>
public class UserService : IUserService
{
    private readonly BackendDbContext _dbContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes the user service with database context and AutoMapper
    /// </summary>
    public UserService(BackendDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves a user by their email address
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <returns>User entity if found, null otherwise</returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    /// <summary>
    /// Retrieves a user by their unique identifier
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User entity if found, null otherwise</returns>
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    /// <summary>
    /// Creates a new user account with hashed password
    /// </summary>
    /// <param name="user">User entity with basic information</param>
    /// <param name="rawPassword">Plain text password to be hashed</param>
    /// <returns>Created user entity with generated ID</returns>
    public async Task<User> CreateAsync(User user, string rawPassword)
    {
        // Set account creation timestamp
        user.CreatedAt = DateTime.UtcNow;

        // Hash password using BCrypt for secure storage
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(rawPassword);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Verifies a password against the stored hash
    /// </summary>
    /// <param name="user">User entity containing the password hash</param>
    /// <param name="password">Plain text password to verify</param>
    /// <returns>True if password matches, false otherwise</returns>
    public bool VerifyPassword(User user, string password)
    {
        // Validate password hash exists
        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            return false;
        }

        // Use BCrypt to verify password against stored hash
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    /// <summary>
    /// Updates an existing user's information
    /// </summary>
    /// <param name="user">User entity with updated information</param>
    /// <returns>Updated user entity</returns>
    public async Task<User> UpdateAsync(User user)
    {
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Updates a user's password securely with hashing
    /// </summary>
    /// <param name="user">User entity to update</param>
    /// <param name="newPassword">New plain text password to hash and store</param>
    /// <returns>Updated user entity</returns>
    public async Task<User> UpdatePasswordAsync(User user, string newPassword)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;
        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    /// <summary>
    /// Retrieves all registered users
    /// </summary>
    /// <returns>Collection of all user entities</returns>
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    /// <summary>
    /// Deletes a user account permanently
    /// </summary>
    /// <param name="id">User ID to delete</param>
    /// <returns>True if deletion successful, false if user not found</returns>
    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null) return false;

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}