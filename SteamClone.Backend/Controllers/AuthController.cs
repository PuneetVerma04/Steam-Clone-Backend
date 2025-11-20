using Microsoft.AspNetCore.Mvc;
using SteamClone.Backend.DTOs.User;
using SteamClone.Backend.DTOs.Auth;
using SteamClone.Backend.Entities;
using SteamClone.Backend.Services;
using AutoMapper;

namespace SteamClone.Backend.Controllers;

/// <summary>
/// Handles user authentication operations including registration and login
/// </summary>
[ApiController]
[Route("store/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly JwtService _jwtService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes the authentication controller with required services
    /// </summary>
    public AuthController(IUserService userService, JwtService jwtService, IMapper mapper)
    {
        _userService = userService;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    /// <summary>
    /// Registers a new user account
    /// </summary>
    /// <param name="registerDto">User registration details including username, email, and password</param>
    /// <returns>JWT token for immediate authentication after successful registration</returns>
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterDto registerDto)
    {
        // Check if email is already in use to prevent duplicate accounts
        if (_userService.GetByEmail(registerDto.Email) is not null)
            return BadRequest("Email already registered");

        // Map DTO to User entity and set default role
        var newUser = _mapper.Map<User>(registerDto);
        newUser.Role = UserRole.Player; // All new registrations default to Player role

        // Create user with hashed password
        var createdUser = _userService.Create(newUser, registerDto.Password);
        if (createdUser == null)
            return StatusCode(500, "User creation failed");

        // Generate JWT token for immediate authentication
        var token = _jwtService.GenerateToken(createdUser);

        return Ok(new { token });
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token
    /// </summary>
    /// <param name="loginDto">Login credentials (email and password)</param>
    /// <returns>Authentication response with JWT token and user details</returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto loginDto)
    {
        // Retrieve user by email
        var user = _userService.GetByEmail(loginDto.Email);

        // Validate user exists
        if (user is null) return Unauthorized("Invalid email");

        // Verify password using BCrypt
        if (!_userService.VerifyPassword(user, loginDto.Password))
            return Unauthorized("Invalid password");

        // Generate JWT token for authenticated session
        var token = _jwtService.GenerateToken(user);
        return Ok(new AuthResponseDto
        {
            Token = token,
            Username = user.Username,
            Role = user.Role.ToString()
        });
    }
}