using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamClone.Backend.DTOs.Game;
using SteamClone.Backend.Services;
using SteamClone.Backend.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SteamClone.Backend.Controllers;

/// <summary>
/// Manages game catalog operations including CRUD operations and filtering
/// </summary>
[ApiController]
[Route("store/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly BackendDbContext _dbContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes the games controller with game service, database context, and mapper
    /// </summary>
    public GamesController(IGameService gameService, BackendDbContext dbContext, IMapper mapper)
    {
        _gameService = gameService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all games with optional filtering, sorting, and pagination
    /// </summary>
    /// <param name="queryParameters">Query parameters for filtering, sorting, and pagination</param>
    /// <returns>Paged collection of games matching the criteria</returns>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedGameResponse>> GetGames([FromQuery] GameQueryParameters queryParameters)
    {
        var result = await _gameService.GetGamesAsync(queryParameters);
        return Ok(result);
    }

    /// <summary>
    /// Legacy endpoint: Retrieves all games with basic filtering (deprecated - use GET with query params instead)
    /// </summary>
    /// <param name="genre">Optional genre filter (case-insensitive)</param>
    /// <param name="maxPrice">Optional maximum price filter</param>
    /// <returns>Collection of games matching the filter criteria</returns>
    [HttpGet("all")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<GameResponseDTO>>> GetAllGames(string? genre = null, decimal? maxPrice = null)
    {
        var query = _dbContext.Games.Include(g => g.Publisher).AsQueryable();

        if (!string.IsNullOrEmpty(genre))
            query = query.Where(g => g.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));

        if (maxPrice.HasValue)
            query = query.Where(g => g.Price <= maxPrice.Value);

        var games = await query.ToListAsync();
        return Ok(_mapper.Map<IEnumerable<GameResponseDTO>>(games));
    }

    /// <summary>
    /// Retrieves a specific game by its ID
    /// </summary>
    /// <param name="id">Game identifier</param>
    /// <returns>Game details if found, otherwise NotFound</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<GameResponseDTO>> GetGameById(int id)
    {
        var game = await _gameService.GetByIdAsync(id);
        if (game == null)
        {
            return NotFound();
        }
        return Ok(game);
    }

    /// <summary>
    /// Creates a new game in the catalog (Publisher and Admin only)
    /// </summary>
    /// <param name="newGame">Game details for creation</param>
    /// <returns>Created game with generated ID and location header</returns>
    [HttpPost]
    [Authorize(Roles = "Publisher,Admin")]
    public async Task<ActionResult<GameResponseDTO>> CreateGame([FromBody] CreateGameRequestDTO newGame)
    {
        var createdGame = await _gameService.CreateGameAsync(newGame);
        return CreatedAtAction(nameof(GetGameById), new { id = createdGame.Id }, createdGame);
    }

    /// <summary>
    /// Updates an existing game (Publisher and Admin only)
    /// Publishers can only update their own games
    /// </summary>
    /// <param name="id">Game identifier to update</param>
    /// <param name="updatedGame">Updated game information</param>
    /// <returns>Updated game details if found and authorized, otherwise NotFound or Forbidden</returns>
    [HttpPatch("{id}")]
    [Authorize(Roles = "Publisher,Admin")]
    public async Task<ActionResult<GameResponseDTO>> UpdateGame(int id, [FromBody] UpdateGameRequestDTO updatedGame)
    {
        // Extract user info from JWT claims
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(userRole) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var updated = await _gameService.UpdateGameAsync(id, updatedGame, userId, userRole);
        if (updated == null)
        {
            return NotFound(); // Could be not found or unauthorized (publisher trying to update someone else's game)
        }
        return Ok(updated);
    }

    /// <summary>
    /// Deletes a game from the catalog (Admin only)
    /// </summary>
    /// <param name="id">Game identifier to delete</param>
    /// <returns>NoContent if successful, NotFound if game doesn't exist</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteGame(int id)
    {
        // Verify game exists before attempting deletion
        var game = await _gameService.GetByIdAsync(id);
        if (game == null)
        {
            return NotFound();
        }

        await _gameService.DeleteGameAsync(id);
        return NoContent();
    }
}