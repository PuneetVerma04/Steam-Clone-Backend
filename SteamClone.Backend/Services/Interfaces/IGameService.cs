using SteamClone.Backend.DTOs.Game;

namespace SteamClone.Backend.Services;

public interface IGameService
{
    IEnumerable<GameResponseDTO> GetAllGames();
    GameResponseDTO? GetById(int id);
    GameResponseDTO CreateGame(CreateGameRequestDTO gameDto);
    GameResponseDTO? UpdateGame(int id, UpdateGameRequestDTO updatedGameDto, int userId, string userRole);
    bool DeleteGame(int id);
}