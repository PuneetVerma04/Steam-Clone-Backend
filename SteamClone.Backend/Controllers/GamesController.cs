using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamClone.Backend.DTOs.Game;
using SteamClone.Backend.Services.Interfaces;
using SteamClone.Backend.Services;

namespace SteamClone.Backend.Controllers;

[ApiController]
[Route("store/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet]
    [AllowAnonymous]
    public ActionResult<IEnumerable<GameResponseDTO>> GetGames(string? genre = null, decimal? maxPrice = null)
    {
        var games = _gameService.GetAllGames();

        if (!string.IsNullOrEmpty(genre))
        {
            games = games.Where(g => g.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));
        }

        if (maxPrice.HasValue)
        {
            games = games.Where(g => g.Price <= maxPrice.Value);
        }

        return Ok(games);
    }

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

    [HttpPost]
    [Authorize(Roles = "Publisher,Admin")]
    public ActionResult<GameResponseDTO> CreateGame([FromBody] CreateGameRequestDTO newGame)
    {
        var createdGame = _gameService.CreateGame(newGame);
        return CreatedAtAction(nameof(GetGameById), new { id = createdGame.Id }, createdGame);
    }

    [HttpPut("{id}")]
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

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public ActionResult DeleteGame(int id)
    {
        var game = _gameService.GetById(id);
        if (game == null)
        {
            return NotFound();
        }

        _gameService.DeleteGame(id);
        return NoContent();
    }
}