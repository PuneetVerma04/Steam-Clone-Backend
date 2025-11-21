namespace SteamClone.Backend.DTOs.Review;

public class ReviewDto
{
    public string Username { get; set; } = null!;
    public string GameTitle { get; set; } = null!;
    public string? Comment { get; set; }
    public int Rating { get; set; }
    public DateTime ReviewDate { get; set; }
}