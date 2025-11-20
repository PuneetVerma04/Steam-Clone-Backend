using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamClone.Backend.DTOs.Cart;
using SteamClone.Backend.Services;
using AutoMapper;
using System.Security.Claims;

namespace SteamClone.Backend.Controllers;

/// <summary>
/// Manages shopping cart operations for authenticated players
/// </summary>
[ApiController]
[Route("store/[controller]")]
[Authorize(Roles = "Player")]
public class CartController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly ICartService _cartService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes the cart controller with required services
    /// </summary>
    public CartController(IGameService gameService, ICartService cartService, IMapper mapper)
    {
        _gameService = gameService;
        _cartService = cartService;
        _mapper = mapper;
    }

    /// <summary>
    /// Extracts the authenticated user's ID from JWT claims
    /// </summary>
    /// <returns>User ID from the authentication token</returns>
    /// <exception cref="Exception">Thrown if User ID claim is missing</exception>
    private int GetUserIdFromClaims()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? throw new Exception("User ID claim missing"));
    }

    /// <summary>
    /// Retrieves all items in the current user's shopping cart
    /// </summary>
    /// <returns>Collection of cart items with game details</returns>
    [HttpGet]
    public IActionResult GetCart()
    {
        var userId = GetUserIdFromClaims();
        var items = _cartService.GetCartItems(userId);
        return Ok(items);
    }

    /// <summary>
    /// Adds a game to the shopping cart or increases quantity if already present
    /// </summary>
    /// <param name="request">Cart request containing game ID and quantity</param>
    /// <returns>Updated cart items after addition</returns>
    [HttpPost("add")]
    public IActionResult AddToCart([FromBody] CartRequest request)
    {
        var userId = GetUserIdFromClaims();

        // Verify game exists before adding to cart
        var game = _gameService.GetById(request.GameId);
        if (game == null)
        {
            return NotFound("Game not found.");
        }

        // Add or update cart item
        _cartService.AddToCart(userId, request.GameId, request.Quantity);
        var items = _cartService.GetCartItems(userId);
        return Ok(items);
    }

    /// <summary>
    /// Updates the quantity of an item in the cart or removes it if quantity is 0
    /// </summary>
    /// <param name="request">Cart request with updated quantity</param>
    /// <returns>Updated cart items after modification</returns>
    [HttpPatch("update")]
    public IActionResult UpdateCartItem([FromBody] CartRequest request)
    {
        var userId = GetUserIdFromClaims();

        // Update quantity or remove item if quantity is 0 or negative
        _cartService.UpdateCartItem(userId, request.GameId, request.Quantity);

        var items = _cartService.GetCartItems(userId);
        return Ok(items);
    }
}