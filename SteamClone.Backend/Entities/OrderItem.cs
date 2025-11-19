namespace SteamClone.Backend.Entities;

public class OrderItem
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; } // Store price at time of purchase
}
