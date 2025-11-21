using System.ComponentModel.DataAnnotations;

namespace SteamClone.Backend.Entities;

/// <summary>
/// Represents a user review for a game
/// Each user can only review a game once (enforced by unique constraint)
/// </summary>
public class Review
{
    /// <summary>Unique identifier for the review</summary>
    public int ReviewId { get; set; }

    /// <summary>ID of the user who wrote the review</summary>
    public int UserId { get; set; }

    /// <summary>ID of the game being reviewed</summary>
    public int GameId { get; set; }

    /// <summary>Navigation property to the user who wrote the review</summary>
    public User User { get; set; } = null!;

    /// <summary>Navigation property to the game being reviewed</summary>
    public Game Game { get; set; } = null!;

    /// <summary>Optional text comment/feedback about the game</summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Numeric rating from 1 (worst) to 5 (best)
    /// </summary>
    [Range(1, 5)]
    public int Rating { get; set; }

    /// <summary>Timestamp when the review was created</summary>
    public DateTime ReviewDate { get; set; } = DateTime.UtcNow;
}