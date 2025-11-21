using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamClone.Backend.DTOs.User;
using SteamClone.Backend.Entities;
using SteamClone.Backend.Services;
using AutoMapper;
using System.Security.Claims;

namespace SteamClone.Backend.Controllers;

/// <summary>
/// Manages user account operations including retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("store/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes the users controller with required services
    /// </summary>
    public UsersController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all users with optional username filtering (Admin only)
    /// </summary>
    /// <param name="username">Optional username search term</param>
    /// <returns>Collection of users matching the filter</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] string? username)
    {
        var users = await _userService.GetAllUsersAsync();

        // Filter by username if search term provided
        if (!string.IsNullOrEmpty(username))
        {
            users = users.Where(u => u.Username.Contains(username, StringComparison.OrdinalIgnoreCase));
        }

        var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
        return Ok(userDtos);
    }

    /// <summary>
    /// Retrieves a specific user by ID (users can access their own data, admins can access any)
    /// </summary>
    /// <param name="id">User identifier</param>
    /// <returns>User details if found and authorized</returns>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // Verify user is accessing their own data or is an admin
        if (currentUserRole != "Admin" && currentUserId != id)
        {
            return Forbid();
        }

        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();

        var userDto = _mapper.Map<UserDto>(user);
        return Ok(userDto);
    }

    /// <summary>
    /// Updates user information (users can update their own data, admins can update any)
    /// </summary>
    /// <param name="id">User identifier to update</param>
    /// <param name="updatedUserDto">Updated user information</param>
    /// <returns>Updated user details if found and authorized</returns>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> UpdatedUser(int id, [FromBody] UpdateUserDto updatedUserDto)
    {
        var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

        // Verify user is updating their own data or is an admin
        if (currentUserRole != "Admin" && currentUserId != id)
        {
            return Forbid();
        }

        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Map updated fields to user entity
        if (updatedUserDto.Username != null)
            user.Username = updatedUserDto.Username;
        if (updatedUserDto.Email != null)
            user.Email = updatedUserDto.Email;

        User updatedUser;
        if (updatedUserDto.Password != null)
        {
            // Use service method for secure password hashing
            updatedUser = await _userService.UpdatePasswordAsync(user, updatedUserDto.Password);
        }
        else
        {
            user.UpdatedAt = DateTime.UtcNow;
            updatedUser = await _userService.UpdateAsync(user);
        }
        var userDto = _mapper.Map<UserDto>(updatedUser);

        return Ok(userDto);
    }

    /// <summary>
    /// Deletes a user account (Admin only)
    /// </summary>
    /// <param name="id">User identifier to delete</param>
    /// <returns>NoContent if successful, NotFound if user doesn't exist</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var success = await _userService.DeleteAsync(id);
        if (!success) return NotFound();

        return NoContent();
    }
}