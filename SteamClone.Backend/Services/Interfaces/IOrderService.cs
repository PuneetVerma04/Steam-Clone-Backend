using SteamClone.Backend.DTOs.Order;
using SteamClone.Backend.Entities;

namespace SteamClone.Backend.Services;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(int userId, List<CartItem> cartItems);
    Task<OrderResponseDto?> GetOrderByIdAsync(int id);
    Task<IEnumerable<OrderResponseDto>> GetOrdersForUserAsync(int userId);
    Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();
    Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
}