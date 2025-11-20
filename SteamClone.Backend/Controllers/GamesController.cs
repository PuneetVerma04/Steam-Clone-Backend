using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamClone.Backend.DTOs.Game;
using SteamClone.Backend.Services;

namespace SteamClone.Backend.Controllers;

/// <summary>
/// Manages game catalog operations including CRUD operations and filtering
/// </summary>
[ApiController]
[Route("store/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    /// <summary>
    /// Initializes the games controller with game service
    /// </summary>
    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    /// <summary>
    /// Retrieves all games with optional filtering by genre and price
    /// </summary>
    /// <param name="genre">Optional genre filter (case-insensitive)</param>
    /// <param name="maxPrice">Optional maximum price filter</param>
    /// <returns>Collection of games matching the filter criteria</returns>
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<IEnumerable<GameResponseDTO>> GetGames(string? genre = null, decimal? maxPrice = null)
    {
        // Retrieve all games from the database
        var games = _gameService.GetAllGames();

        // Apply genre filter if provided
        if (!string.IsNullOrEmpty(genre))
        {
            games = games.Where(g => g.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));
        }

        // Apply price filter if provided
        if (maxPrice.HasValue)
        {
            games = games.Where(g => g.Price <= maxPrice.Value);
        }

        return Ok(games);
    }

    /// <summary>
    /// Retrieves a specific game by its ID
    /// </summary>
    /// <param name="id">Game identifier</param>
    /// <returns>Game details if found, otherwise NotFound</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public ActionResult<GameResponseDTO> GetGameById(int id)
    {
        var game = _gameService.GetById(id);
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
    public ActionResult<GameResponseDTO> CreateGame([FromBody] CreateGameRequestDTO newGame)
    {
        var createdGame = _gameService.CreateGame(newGame);
        return CreatedAtAction(nameof(GetGameById), new { id = createdGame.Id }, createdGame);
    }

    /// <summary>
    /// Updates an existing game (Publisher and Admin only)
    /// </summary>
    /// <param name="id">Game identifier to update</param>
    /// <param name="updatedGame">Updated game information</param>
    /// <returns>Updated game details if found, otherwise NotFound</returns>
    [HttpPatch("{id}")]
    [Authorize(Roles = "Publisher,Admin")]
    public ActionResult<GameResponseDTO> UpdateGame(int id, [FromBody] UpdateGameRequestDTO updatedGame)
    {
        var updated = _gameService.UpdateGame(id, updatedGame);
        if (updated == null)
        {
            return NotFound();
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
    public ActionResult DeleteGame(int id)
    {
        // Verify game exists before attempting deletion
        var game = _gameService.GetById(id);
        if (game == null)
        {
            return NotFound();
        }

        _gameService.DeleteGame(id);
        return NoContent();
    }
}