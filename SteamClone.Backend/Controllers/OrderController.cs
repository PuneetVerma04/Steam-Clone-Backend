using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamClone.Backend.DTOs.Order;
using SteamClone.Backend.Entities;
using SteamClone.Backend.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SteamClone.Backend.Controllers;

/// <summary>
/// Manages order operations including checkout, order history, and status updates
/// </summary>
[ApiController]
[Route("store/[controller]")]
[Authorize(Roles = "Player,Admin")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;
    private readonly IMapper _mapper;
    private readonly BackendDbContext _dbContext;

    /// <summary>
    /// Initializes the order controller with required services
    /// </summary>
    public OrderController(IOrderService orderService, ICartService cartService, IMapper mapper, BackendDbContext dbContext)
    {
        _orderService = orderService;
        _cartService = cartService;
        _mapper = mapper;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Extracts the authenticated user's ID from JWT claims
    /// </summary>
    /// <returns>User ID from the authentication token</returns>
    /// <exception cref="Exception">Thrown if User ID claim is missing</exception>
    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User ID Claim missing"));
    }

    /// <summary>
    /// Extracts the authenticated user's role from JWT claims
    /// </summary>
    /// <returns>User role from the authentication token</returns>
    /// <exception cref="Exception">Thrown if User Role claim is missing</exception>
    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? throw new Exception("User Role Claim missing");
    }

    /// <summary>
    /// Retrieves orders for the current user, or all orders if user is Admin
    /// </summary>
    /// <returns>Collection of orders based on user role</returns>
    [HttpGet]
    public async Task<IActionResult> GetOrdersforCurrentUser()
    {
        var currentUserId = GetCurrentUserId();
        var currentUserRole = GetCurrentUserRole();

        // Admins can view all orders, regular users only see their own
        var orders = currentUserRole == "Admin"
                    ? await _orderService.GetAllOrdersAsync()
                    : await _orderService.GetOrdersForUserAsync(currentUserId);

        return Ok(orders);
    }

    /// <summary>
    /// Processes checkout by creating an order from cart items
    /// </summary>
    /// <returns>Created order details with order ID</returns>
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
    {
        var currentUserId = GetCurrentUserId();

        // Fetch actual CartItem entities with Game navigation property
        var cartItemEntities = await _dbContext.CartItems
            .Include(ci => ci.Game)
            .Where(ci => ci.UserId == currentUserId)
            .ToListAsync();

        // Validate cart is not empty
        if (!cartItemEntities.Any())
        {
            return BadRequest("Cart is empty");
        }

        // Create order from cart items and clear the cart
        var order = await _orderService.CreateOrderAsync(currentUserId, cartItemEntities);
        await _cartService.ClearCartAsync(currentUserId);
        return Ok(order);
    }

    /// <summary>
    /// Retrieves detailed information for a specific order
    /// </summary>
    /// <param name="orderId">Order identifier</param>
    /// <returns>Order details if found and authorized, otherwise NotFound or Forbidden</returns>
    [HttpGet("{orderId}")]
    [Authorize(Roles = "Player,Admin")]
    public async Task<IActionResult> GetOrderDetails(int orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return NotFound("Order not found");
        }

        var currentUserId = GetCurrentUserId();
        var currentUserRole = GetCurrentUserRole();

        // Verify user owns the order or is an admin
        if (currentUserRole != "Admin" && order.UserId != currentUserId)
        {
            return Forbid();
        }
        return Ok(order);
    }

    /// <summary>
    /// Updates the status of an order (Admin only)
    /// </summary>
    /// <param name="orderId">Order identifier to update</param>
    /// <param name="newStatus">New order status</param>
    /// <returns>Ok if successful, NotFound if order doesn't exist</returns>
    [HttpPatch("{orderId}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] OrderStatus newStatus)
    {
        var success = await _orderService.UpdateOrderStatusAsync(orderId, newStatus);
        if (!success)
        {
            return NotFound("Order not found");
        }
        return Ok();
    }
}