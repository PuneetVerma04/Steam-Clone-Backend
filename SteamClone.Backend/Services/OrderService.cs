using SteamClone.Backend.Entities;
using SteamClone.Backend.DTOs.Order;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SteamClone.Backend.Services;

public class OrderService : IOrderService
{
    private readonly BackendDbContext _dbContext;
    private readonly IMapper _mapper;

    public OrderService(BackendDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public OrderResponseDto CreateOrder(int userId, List<CartItem> cartItems)
    {
        var order = new Order
        {
            UserId = userId,
            Items = new List<CartItem>(cartItems),
            TotalPrice = cartItems.Sum(item => item.Game.Price * item.Quantity),
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Completed
        };

        _dbContext.Orders.Add(order);
        _dbContext.SaveChanges();

        return _mapper.Map<OrderResponseDto>(order);
    }

    public OrderResponseDto? GetOrderById(int orderId)
    {
        var order = _dbContext.Orders.Find(orderId);
        return order == null ? null : _mapper.Map<OrderResponseDto>(order);
    }

    public IEnumerable<OrderResponseDto> GetOrdersForUser(int userId)
    {
        var orders = _dbContext.Orders
            .Where(o => o.UserId == userId)
            .ToList();
        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public IEnumerable<OrderResponseDto> GetAllOrders()
    {
        var orders = _dbContext.Orders.ToList();
        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public bool UpdateOrderStatus(int orderId, OrderStatus newStatus)
    {
        var order = _dbContext.Orders.Find(orderId);
        if (order == null) return false;

        order.Status = newStatus;
        _dbContext.SaveChanges();

        return true;
    }
}
