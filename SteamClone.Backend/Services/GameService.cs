using SteamClone.Backend.Entities;
using SteamClone.Backend.DTOs.Game;
using AutoMapper;
using Microsoft.EntityFrameworkCore;


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
        var games = _dbContext.Games
            .Include(g => g.Publisher)
            .ToList();
        return _mapper.Map<IEnumerable<GameResponseDTO>>(games);
    }

    /// <summary>
    /// Retrieves a specific game by its identifier
    /// </summary>
    /// <param name="id">Game ID</param>
    /// <returns>Game DTO if found, null otherwise</returns>
    public GameResponseDTO? GetById(int id)
    {
        var game = _dbContext.Games
            .Include(g => g.Publisher)
            .FirstOrDefault(g => g.Id == id);
        return game == null ? null : _mapper.Map<GameResponseDTO>(game);
    }

    /// <summary>
    /// Creates a new game in the catalog
    /// </summary>
    /// <param name="gameDto">Game creation request with all required details</param>
    /// <returns>Created game DTO with generated ID</returns>
    public GameResponseDTO CreateGame(CreateGameRequestDTO gameDto)
    {
        var publisher = _dbContext.Users
            .FirstOrDefault(u => u.Id == gameDto.PublisherId && u.Role == UserRole.Publisher);


        if (publisher == null)
        {
            throw new ArgumentException("Invalid PublisherId: No such publisher exists.");
        }
        // Map DTO to entity
        var game = _mapper.Map<Game>(gameDto);
        game.Publisher = publisher;

        _dbContext.Games.Add(game);
        _dbContext.SaveChanges();

        _dbContext.Entry(game).Reference(g => g.Publisher).Load();
        return _mapper.Map<GameResponseDTO>(game);
    }

    /// <summary>
    /// Updates an existing game's information
    /// Publishers can only update their own games, Admins can update any game and change publishers
    /// </summary>
    /// <param name="id">Game ID to update</param>
    /// <param name="updatedGameDto">Updated game information</param>
    /// <param name="userId">ID of the user making the update</param>
    /// <param name="userRole">Role of the user making the update</param>
    /// <returns>Updated game DTO if found and authorized, null otherwise</returns>
    public GameResponseDTO? UpdateGame(int id, UpdateGameRequestDTO updatedGameDto, int userId, string userRole)
    {
        var existingGame = _dbContext.Games
            .Include(g => g.Publisher)
            .FirstOrDefault(g => g.Id == id);

        if (existingGame == null) return null;

        // Authorization: Publishers can only update their own games
        if (userRole == "Publisher" && existingGame.PublisherId != userId)
        {
            return null; // Unauthorized - not the owner
        }

        // Store the requested PublisherId before mapping
        int? requestedPublisherId = updatedGameDto.PublisherId;

        if (userRole == "Publisher")
        {
            // Publishers cannot change publisherId (ownership)
            updatedGameDto.PublisherId = null;
        }
        else if (userRole == "Admin" && requestedPublisherId.HasValue)
        {
            var newPub = _dbContext.Users
                .FirstOrDefault(u => u.Id == requestedPublisherId.Value && u.Role == UserRole.Publisher);
            if (newPub == null) return null; // Invalid publisher
            // Will be applied via mapping: ensure PublisherId present
        }
        else
        {
            // Other roles shouldn't be able to update - controller authorizes to Publisher,Admin only
            updatedGameDto.PublisherId = null;
        }

        // Map updated fields to existing entity
        _mapper.Map(updatedGameDto, existingGame);
        _dbContext.SaveChanges();

        // Reload to get updated Publisher info
        var updated = _dbContext.Games
            .Include(g => g.Publisher)
            .FirstOrDefault(g => g.Id == id);

        return _mapper.Map<GameResponseDTO>(updated);
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
