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
    public async Task<IEnumerable<GameResponseDTO>> GetAllGamesAsync()
    {
        var games = await _dbContext.Games
            .Include(g => g.Publisher)
            .ToListAsync();
        return _mapper.Map<IEnumerable<GameResponseDTO>>(games);
    }

    /// <summary>
    /// Retrieves games with pagination, filtering, and sorting
    /// </summary>
    /// <param name="queryParameters">Query parameters for filtering, sorting, and pagination</param>
    /// <returns>Paged response with games and metadata</returns>
    public async Task<PagedGameResponse> GetGamesAsync(GameQueryParameters queryParameters)
    {
        queryParameters.ValidateAndAdjust();

        // Start with base query
        var query = _dbContext.Games.Include(g => g.Publisher).AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(queryParameters.Genre))
        {
            query = query.Where(g => g.Genre.ToLower().Contains(queryParameters.Genre.ToLower()));
        }

        if (!string.IsNullOrEmpty(queryParameters.SearchTerm))
        {
            query = query.Where(g => g.Title.ToLower().Contains(queryParameters.SearchTerm.ToLower()) ||
                                    g.Description.ToLower().Contains(queryParameters.SearchTerm.ToLower()));
        }

        if (queryParameters.MinPrice.HasValue)
        {
            query = query.Where(g => g.Price >= queryParameters.MinPrice.Value);
        }

        if (queryParameters.MaxPrice.HasValue)
        {
            query = query.Where(g => g.Price <= queryParameters.MaxPrice.Value);
        }

        if (queryParameters.PublisherId.HasValue)
        {
            query = query.Where(g => g.PublisherId == queryParameters.PublisherId.Value);
        }

        if (queryParameters.ReleaseDateFrom.HasValue)
        {
            query = query.Where(g => g.ReleaseDate >= queryParameters.ReleaseDateFrom.Value);
        }

        if (queryParameters.ReleaseDateTo.HasValue)
        {
            query = query.Where(g => g.ReleaseDate <= queryParameters.ReleaseDateTo.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = ApplySorting(query, queryParameters.SortBy, queryParameters.SortOrder);

        // Apply pagination
        var games = await query
            .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
            .Take(queryParameters.PageSize)
            .ToListAsync();

        var gameDtos = _mapper.Map<IEnumerable<GameResponseDTO>>(games);

        return new PagedGameResponse
        {
            Games = gameDtos,
            PageNumber = queryParameters.PageNumber,
            PageSize = queryParameters.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)queryParameters.PageSize)
        };
    }

    /// <summary>
    /// Applies sorting to the game query
    /// </summary>
    private IQueryable<Game> ApplySorting(IQueryable<Game> query, string? sortBy, string sortOrder)
    {
        var isDescending = sortOrder.ToLower() == "desc";

        return sortBy?.ToLower() switch
        {
            "title" => isDescending ? query.OrderByDescending(g => g.Title) : query.OrderBy(g => g.Title),
            "price" => isDescending ? query.OrderByDescending(g => g.Price) : query.OrderBy(g => g.Price),
            "releasedate" => isDescending ? query.OrderByDescending(g => g.ReleaseDate) : query.OrderBy(g => g.ReleaseDate),
            "genre" => isDescending ? query.OrderByDescending(g => g.Genre) : query.OrderBy(g => g.Genre),
            _ => query.OrderBy(g => g.Id) // Default sorting by ID
        };
    }

    /// <summary>
    /// Retrieves a specific game by its identifier
    /// </summary>
    /// <param name="id">Game ID</param>
    /// <returns>Game DTO if found, null otherwise</returns>
    public async Task<GameResponseDTO?> GetByIdAsync(int id)
    {
        var game = await _dbContext.Games
            .Include(g => g.Publisher)
            .FirstOrDefaultAsync(g => g.Id == id);
        return game == null ? null : _mapper.Map<GameResponseDTO>(game);
    }

    /// <summary>
    /// Creates a new game in the catalog
    /// </summary>
    /// <param name="gameDto">Game creation request with all required details</param>
    /// <returns>Created game DTO with generated ID</returns>
    public async Task<GameResponseDTO> CreateGameAsync(CreateGameRequestDTO gameDto)
    {
        var publisher = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == gameDto.PublisherId && u.Role == UserRole.Publisher);


        if (publisher == null)
        {
            throw new ArgumentException("Invalid PublisherId: No such publisher exists.");
        }
        // Map DTO to entity
        var game = _mapper.Map<Game>(gameDto);
        game.Publisher = publisher;

        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();

        await _dbContext.Entry(game).Reference(g => g.Publisher).LoadAsync();
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
    public async Task<GameResponseDTO?> UpdateGameAsync(int id, UpdateGameRequestDTO updatedGameDto, int userId, string userRole)
    {
        var existingGame = await _dbContext.Games
            .Include(g => g.Publisher)
            .FirstOrDefaultAsync(g => g.Id == id);

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
            var newPub = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == requestedPublisherId.Value && u.Role == UserRole.Publisher);
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
        await _dbContext.SaveChangesAsync();

        // Reload to get updated Publisher info
        var updated = await _dbContext.Games
            .Include(g => g.Publisher)
            .FirstOrDefaultAsync(g => g.Id == id);

        return _mapper.Map<GameResponseDTO>(updated);
    }

    /// <summary>
    /// Removes a game from the catalog
    /// </summary>
    /// <param name="id">Game ID to delete</param>
    /// <returns>True if deletion successful, false if game not found</returns>
    public async Task<bool> DeleteGameAsync(int id)
    {
        var gameToDelete = await _dbContext.Games.FindAsync(id);
        if (gameToDelete == null) return false;

        _dbContext.Games.Remove(gameToDelete);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}

