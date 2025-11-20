using System.ComponentModel.DataAnnotations;

namespace SteamClone.Backend.Entities;

/// <summary>
/// Represents a user review for a game
/// Each user can only review a game once (enforced by unique constraint)
/// </summary>
public class Reviews
{
    /// <summary>Unique identifier for the review</summary>
    public int ReviewId { get; set; }

    /// <summary>ID of the user who wrote the review</summary>
    public required int UserId { get; set; }

    /// <summary>ID of the game being reviewed</summary>
    public required int GameId { get; set; }

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