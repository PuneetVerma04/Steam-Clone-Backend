using SteamClone.Backend.Entities;
using SteamClone.Backend.DTOs.Order;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SteamClone.Backend.Services;

/// <summary>
/// Service for managing order operations including creation, retrieval, and status updates
/// </summary>
public class OrderService : IOrderService
{
    private readonly BackendDbContext _dbContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes the order service with database context and AutoMapper
    /// </summary>
    public OrderService(BackendDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new order from cart items and calculates total price
    /// </summary>
    /// <param name="userId">User ID placing the order</param>
    /// <param name="cartItems">Collection of cart items to convert to order items</param>
    /// <returns>Created order DTO with complete details</returns>
    public OrderResponseDto CreateOrder(int userId, List<CartItem> cartItems)
    {
        // Create order with items and calculate total
        var order = new Order
        {
            UserId = userId,
            Items = cartItems.Select(ci => new OrderItem
            {
                GameId = ci.GameId,
                Quantity = ci.Quantity,
                Price = ci.Game.Price // Store price at time of purchase to preserve historical pricing
            }).ToList(),
            TotalPrice = cartItems.Sum(item => item.Game.Price * item.Quantity),
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Completed
        };

        _dbContext.Orders.Add(order);
        _dbContext.SaveChanges();

        // Reload order with navigation properties for complete response
        var createdOrder = _dbContext.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Game)
            .FirstOrDefault(o => o.OrderId == order.OrderId);

        return _mapper.Map<OrderResponseDto>(createdOrder);
    }

    /// <summary>
    /// Retrieves a specific order by its identifier with full details
    /// </summary>
    /// <param name="orderId">Order ID</param>
    /// <returns>Order DTO if found, null otherwise</returns>
    public OrderResponseDto? GetOrderById(int orderId)
    {
        // Include order items and their associated games
        var order = _dbContext.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Game)
            .FirstOrDefault(o => o.OrderId == orderId);
        return order == null ? null : _mapper.Map<OrderResponseDto>(order);
    }

    /// <summary>
    /// Retrieves all orders for a specific user
    /// </summary>
    /// <param name="userId">User ID whose orders to retrieve</param>
    /// <returns>Collection of order DTOs with complete information</returns>
    public IEnumerable<OrderResponseDto> GetOrdersForUser(int userId)
    {
        var orders = _dbContext.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Game)
            .Where(o => o.UserId == userId)
            .ToList();
        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    /// <summary>
    /// Retrieves all orders in the system (admin function)
    /// </summary>
    /// <returns>Collection of all order DTOs</returns>
    public IEnumerable<OrderResponseDto> GetAllOrders()
    {
        var orders = _dbContext.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Game)
            .ToList();
        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    /// <summary>
    /// Updates the status of an order (e.g., Pending, Completed, Cancelled)
    /// </summary>
    /// <param name="orderId">Order ID to update</param>
    /// <param name="newStatus">New order status</param>
    /// <returns>True if update successful, false if order not found</returns>
    public bool UpdateOrderStatus(int orderId, OrderStatus newStatus)
    {
        var order = _dbContext.Orders.Find(orderId);
        if (order == null) return false;

        order.Status = newStatus;
        _dbContext.SaveChanges();

        return true;
    }
}
