using SteamClone.Backend.Entities;
using SteamClone.Backend.DTOs.Cart;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SteamClone.Backend.Services;

public class CartService : ICartService
{
    private readonly BackendDbContext _dbContext;
    private readonly IMapper _mapper;

    public CartService(BackendDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public IEnumerable<CartItemDto> GetCartItems(int userId)
    {
        var items = _dbContext.CartItems
            .Include(ci => ci.Game)
            .Where(item => item.UserId == userId)
            .ToList();
        return _mapper.Map<IEnumerable<CartItemDto>>(items);
    }

    public void AddToCart(int userId, int gameId, int quantity)
    {
        var existingItem = _dbContext.CartItems
            .FirstOrDefault(item => item.UserId == userId && item.GameId == gameId);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            var game = _dbContext.Games.Find(gameId);
            if (game == null)
            {
                throw new Exception("Game not found");
            }

            _dbContext.CartItems.Add(new CartItem
            {
                UserId = userId,
                GameId = gameId,
                Quantity = quantity,
                Game = game
            });
        }

        _dbContext.SaveChanges();
    }

    public void UpdateCartItem(int userId, int gameId, int quantity)
    {
        var existingItem = _dbContext.CartItems
            .FirstOrDefault(item => item.UserId == userId && item.GameId == gameId);

        if (existingItem == null)
        {
            throw new Exception("Item not found in cart");
        }

        if (quantity > 0)
        {
            existingItem.Quantity = quantity;
            _dbContext.SaveChanges();
        }
        else
        {
            _dbContext.CartItems.Remove(existingItem);
            _dbContext.SaveChanges();
        }
    }

    public void RemoveCartItem(int userId, int gameId)
    {
        var existingItem = _dbContext.CartItems
            .FirstOrDefault(item => item.UserId == userId && item.GameId == gameId);

        if (existingItem != null)
        {
            _dbContext.CartItems.Remove(existingItem);
            _dbContext.SaveChanges();
        }
    }

    public void ClearCart(int userId)
    {
        var items = _dbContext.CartItems.Where(ci => ci.UserId == userId);
        _dbContext.CartItems.RemoveRange(items);
        _dbContext.SaveChanges();
    }
}
