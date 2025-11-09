using SteamClone.Backend.Entities;
using SteamClone.Backend.DTOs.Game;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SteamClone.Backend.Services;

public class GameService : IGameService
{
    private readonly BackendDbContext _dbContext;
    private readonly IMapper _mapper;

    public GameService(BackendDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public IEnumerable<GameResponseDTO> GetAllGames()
    {
        var games = _dbContext.Games.ToList();
        return _mapper.Map<IEnumerable<GameResponseDTO>>(games);
    }

    public GameResponseDTO? GetById(int id)
    {
        var game = _dbContext.Games.Find(id);
        return game == null ? null : _mapper.Map<GameResponseDTO>(game);
    }

    public GameResponseDTO CreateGame(CreateGameRequestDTO gameDto)
    {
        var game = _mapper.Map<Game>(gameDto);

        _dbContext.Games.Add(game);
        _dbContext.SaveChanges();

        return _mapper.Map<GameResponseDTO>(game);
    }

    public GameResponseDTO? UpdateGame(int id, UpdateGameRequestDTO updatedGameDto)
    {
        var existingGame = _dbContext.Games.Find(id);
        if (existingGame == null) return null;

        _mapper.Map(updatedGameDto, existingGame);
        _dbContext.SaveChanges();

        return _mapper.Map<GameResponseDTO>(existingGame);
    }

    public bool DeleteGame(int id)
    {
        var gameToDelete = _dbContext.Games.Find(id);
        if (gameToDelete == null) return false;

        _dbContext.Games.Remove(gameToDelete);
        _dbContext.SaveChanges();

        return true;
    }
}
