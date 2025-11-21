using SteamClone.Backend.DTOs.Game;

namespace SteamClone.Backend.Services;

public interface IGameService
{
    Task<IEnumerable<GameResponseDTO>> GetAllGamesAsync();
    Task<PagedGameResponse> GetGamesAsync(GameQueryParameters queryParameters);
    Task<GameResponseDTO?> GetByIdAsync(int id);
    Task<GameResponseDTO> CreateGameAsync(CreateGameRequestDTO gameDto);
    Task<GameResponseDTO?> UpdateGameAsync(int id, UpdateGameRequestDTO updatedGameDto, int userId, string userRole);
    Task<bool> DeleteGameAsync(int id);
}