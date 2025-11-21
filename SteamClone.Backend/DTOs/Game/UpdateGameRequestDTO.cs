namespace SteamClone.Backend.DTOs.Game;

public class UpdateGameRequestDTO
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? Genre { get; set; }
    public int? PublisherId { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? ImageUrl { get; set; }
}