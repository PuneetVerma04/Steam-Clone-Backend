namespace SteamClone.Backend.Entities;

/// <summary>
/// Represents a game item in a user's shopping cart
/// Uses composite key (UserId, GameId) to prevent duplicate entries
/// </summary>
public class CartItem
{
    /// <summary>ID of the game in the cart</summary>
    public int GameId { get; set; }

    /// <summary>Navigation property to the associated game entity</summary>
    public required Game Game { get; set; }

    /// <summary>ID of the user who owns this cart item</summary>
    public int UserId { get; set; }

    // Note: User navigation property is commented out to avoid circular reference issues
    // public required User User { get; set; }

    /// <summary>Number of copies of this game in the cart</summary>
    public int Quantity { get; set; }
}