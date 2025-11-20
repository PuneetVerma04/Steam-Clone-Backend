namespace SteamClone.Backend.Entities;

/// <summary>
/// Represents a single game purchase within an order
/// Stores historical price data at the time of purchase
/// </summary>
public class OrderItem
{
    /// <summary>Unique identifier for this order item</summary>
    public int OrderItemId { get; set; }

    /// <summary>ID of the parent order</summary>
    public int OrderId { get; set; }

    /// <summary>Navigation property to the parent order</summary>
    public Order Order { get; set; } = null!;

    /// <summary>ID of the game that was purchased</summary>
    public int GameId { get; set; }

    /// <summary>Navigation property to the game entity</summary>
    public Game Game { get; set; } = null!;

    /// <summary>Number of copies purchased</summary>
    public int Quantity { get; set; }

    /// <summary>Price per unit at the time of purchase (preserves historical pricing)</summary>
    public decimal Price { get; set; }
}
