namespace SteamClone.Backend.Entities;

/// <summary>
/// Represents a game in the store catalog
/// </summary>
public class Game
{
    /// <summary>Unique identifier for the game</summary>
    public int Id { get; set; }

    /// <summary>Game title/name</summary>
    public required string Title { get; set; }

    /// <summary>Detailed description of the game</summary>
    public required string Description { get; set; }

    /// <summary>Current price in the store currency</summary>
    public required decimal Price { get; set; }

    /// <summary>Game genre/category (e.g., RPG, Action, Strategy)</summary>
    public required string Genre { get; set; }

    /// <summary>Name of the game publisher/developer</summary>
    public required string Publisher { get; set; }

    /// <summary>Official release date of the game</summary>
    public required DateTime ReleaseDate { get; set; }

    /// <summary>URL to the game's cover image or thumbnail</summary>
    public required string ImageUrl { get; set; }
}