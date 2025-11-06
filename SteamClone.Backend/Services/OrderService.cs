using SteamClone.Backend.Entities;
using SteamClone.Backend.DTOs.Order;
using AutoMapper;

namespace SteamClone.Backend.Services;

public class OrderService : IOrderService
{
    private readonly List<Order> _orders = new();
    private readonly IMapper _mapper;

    public OrderService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public OrderResponseDto CreateOrder(int userId, List<CartItem> cartItems)
    {
        var order = new Order
        {
            OrderId = _orders.Count + 1,
            UserId = userId,
            Items = new List<CartItem>(cartItems),
            TotalPrice = cartItems.Sum(item => item.Game.Price * item.Quantity),
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Completed
        };
        _orders.Add(order);
        return _mapper.Map<OrderResponseDto>(order);
    }

    public OrderResponseDto? GetOrderById(int orderId)
    {
        var order = _orders.FirstOrDefault(o => o.OrderId == orderId);
        return order == null ? null : _mapper.Map<OrderResponseDto>(order);
    }

    public IEnumerable<OrderResponseDto> GetOrdersForUser(int userId)
    {
        var orders = _orders.Where(o => o.UserId == userId);
        return _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
    }

    public IEnumerable<OrderResponseDto> GetAllOrders()
    {
        return _mapper.Map<IEnumerable<OrderResponseDto>>(_orders);
    }

    public bool UpdateOrderStatus(int orderId, OrderStatus newStatus)
    {
        var order = _orders.FirstOrDefault(o => o.OrderId == orderId);
        if (order == null) return false;
        order.Status = newStatus;
        return true;
    }
}
