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
    public required Game Game { get; set; } = null!;

    /// <summary>ID of the user who owns this cart item</summary>
    public int UserId { get; set; }

    /// <summary>Navigation property to the user who owns this cart item</summary>
    public User User { get; set; } = null!;

    /// <summary>Number of copies of this game in the cart</summary>
    public int Quantity { get; set; }
}