namespace SteamClone.Backend.Entities;

public class CartItem
{
    public int GameId { get; set; }
    public required Game Game { get; set; }
    public int UserId { get; set; }
    //public required User User { get; set; }
    public int Quantity { get; set; }
}