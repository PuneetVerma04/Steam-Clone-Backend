using SteamClone.Backend.DTOs.Cart;
using SteamClone.Backend.Entities;

namespace SteamClone.Backend.Services;

public interface ICartService
{
    Task<IEnumerable<CartItemDto>> GetCartItemsAsync(int userId);
    Task AddToCartAsync(int userId, int gameId, int quantity);
    Task UpdateCartItemAsync(int userId, int gameId, int quantity);
    Task RemoveCartItemAsync(int userId, int gameId);
    Task ClearCartAsync(int userId);
}