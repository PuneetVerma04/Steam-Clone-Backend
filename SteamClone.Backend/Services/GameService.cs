using SteamClone.Backend.Entities;
using SteamClone.Backend.DTOs.Game;
using AutoMapper;


namespace SteamClone.Backend.Services;

/// <summary>
/// Service for managing game catalog operations including CRUD functionality
/// </summary>
public class GameService : IGameService
{
    private readonly BackendDbContext _dbContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes the game service with database context and AutoMapper
    /// </summary>
    public GameService(BackendDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all games from the catalog
    /// </summary>
    /// <returns>Collection of game DTOs with complete information</returns>
    public IEnumerable<GameResponseDTO> GetAllGames()
    {
        var games = _dbContext.Games.ToList();
        return _mapper.Map<IEnumerable<GameResponseDTO>>(games);
    }

    /// <summary>
    /// Retrieves a specific game by its identifier
    /// </summary>
    /// <param name="id">Game ID</param>
    /// <returns>Game DTO if found, null otherwise</returns>
    public GameResponseDTO? GetById(int id)
    {
        var game = _dbContext.Games.Find(id);
        return game == null ? null : _mapper.Map<GameResponseDTO>(game);
    }

    /// <summary>
    /// Creates a new game in the catalog
    /// </summary>
    /// <param name="gameDto">Game creation request with all required details</param>
    /// <returns>Created game DTO with generated ID</returns>
    public GameResponseDTO CreateGame(CreateGameRequestDTO gameDto)
    {
        // Map DTO to entity
        var game = _mapper.Map<Game>(gameDto);

        _dbContext.Games.Add(game);
        _dbContext.SaveChanges();

        return _mapper.Map<GameResponseDTO>(game);
    }

    /// <summary>
    /// Updates an existing game's information
    /// </summary>
    /// <param name="id">Game ID to update</param>
    /// <param name="updatedGameDto">Updated game information</param>
    /// <returns>Updated game DTO if found, null otherwise</returns>
    public GameResponseDTO? UpdateGame(int id, UpdateGameRequestDTO updatedGameDto)
    {
        var existingGame = _dbContext.Games.Find(id);
        if (existingGame == null) return null;

        // Map updated fields to existing entity
        _mapper.Map(updatedGameDto, existingGame);
        _dbContext.SaveChanges();

        return _mapper.Map<GameResponseDTO>(existingGame);
    }

    /// <summary>
    /// Removes a game from the catalog
    /// </summary>
    /// <param name="id">Game ID to delete</param>
    /// <returns>True if deletion successful, false if game not found</returns>
    public bool DeleteGame(int id)
    {
        var gameToDelete = _dbContext.Games.Find(id);
        if (gameToDelete == null) return false;

        _dbContext.Games.Remove(gameToDelete);
        _dbContext.SaveChanges();

        return true;
    }
}
