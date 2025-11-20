namespace SteamClone.Backend.DTOs.Cart;

public class CartItemDto
{
    public int GameId { get; set; }
    public string? Title { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
}