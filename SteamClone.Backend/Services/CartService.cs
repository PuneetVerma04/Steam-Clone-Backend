using SteamClone.Backend.Entities;
using SteamClone.Backend.DTOs.Cart;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SteamClone.Backend.Services;

/// <summary>
/// Service for managing shopping cart operations including adding, updating, and removing items
/// </summary>
public class CartService : ICartService
{
    private readonly BackendDbContext _dbContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes the cart service with database context and AutoMapper
    /// </summary>
    public CartService(BackendDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all items in a user's shopping cart with associated game information
    /// </summary>
    /// <param name="userId">User ID whose cart to retrieve</param>
    /// <returns>Collection of cart items with game details</returns>
    public IEnumerable<CartItemDto> GetCartItems(int userId)
    {
        // Include related Game entity to provide complete cart item information
        var items = _dbContext.CartItems
            .Include(ci => ci.Game)
            .Where(item => item.UserId == userId)
            .ToList();
        return _mapper.Map<IEnumerable<CartItemDto>>(items);
    }

    /// <summary>
    /// Adds a game to the cart or increases quantity if already present
    /// </summary>
    /// <param name="userId">User ID adding the item</param>
    /// <param name="gameId">Game ID to add</param>
    /// <param name="quantity">Number of copies to add</param>
    /// <exception cref="Exception">Thrown if game is not found</exception>
    public void AddToCart(int userId, int gameId, int quantity)
    {
        // Check if item already exists in cart
        var existingItem = _dbContext.CartItems
            .FirstOrDefault(item => item.UserId == userId && item.GameId == gameId);

        if (existingItem != null)
        {
            // Update quantity if item already in cart
            existingItem.Quantity += quantity;
        }
        else
        {
            // Verify game exists before adding to cart
            var game = _dbContext.Games.Find(gameId);
            if (game == null)
            {
                throw new Exception("Game not found");
            }

            // Create new cart item
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

    /// <summary>
    /// Updates the quantity of an item in the cart or removes it if quantity is 0 or less
    /// </summary>
    /// <param name="userId">User ID whose cart to update</param>
    /// <param name="gameId">Game ID to update</param>
    /// <param name="quantity">New quantity (0 or less removes the item)</param>
    /// <exception cref="Exception">Thrown if item not found in cart</exception>
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
            // Update to new quantity
            existingItem.Quantity = quantity;
            _dbContext.SaveChanges();
        }
        else
        {
            // Remove item if quantity is 0 or negative
            _dbContext.CartItems.Remove(existingItem);
            _dbContext.SaveChanges();
        }
    }

    /// <summary>
    /// Removes a specific item from the cart
    /// </summary>
    /// <param name="userId">User ID whose cart to modify</param>
    /// <param name="gameId">Game ID to remove</param>
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

    /// <summary>
    /// Clears all items from a user's cart (typically after checkout)
    /// </summary>
    /// <param name="userId">User ID whose cart to clear</param>
    public void ClearCart(int userId)
    {
        var items = _dbContext.CartItems.Where(ci => ci.UserId == userId);
        _dbContext.CartItems.RemoveRange(items);
        _dbContext.SaveChanges();
    }
}
