namespace SteamClone.Backend.DTOs.Game;

/// <summary>
/// Query parameters for filtering, sorting, and pagination of games
/// </summary>
public class GameQueryParameters
{
  // Pagination
  /// <summary>Page number (starts from 1)</summary>
  public int PageNumber { get; set; } = 1;

  /// <summary>Number of items per page (max 100)</summary>
  public int PageSize { get; set; } = 10;

  // Filtering
  /// <summary>Filter by genre (case-insensitive)</summary>
  public string? Genre { get; set; }

  /// <summary>Filter by minimum price</summary>
  public decimal? MinPrice { get; set; }

  /// <summary>Filter by maximum price</summary>
  public decimal? MaxPrice { get; set; }

  /// <summary>Search by title (case-insensitive partial match)</summary>
  public string? SearchTerm { get; set; }

  /// <summary>Filter by publisher ID</summary>
  public int? PublisherId { get; set; }

  /// <summary>Filter by release date (games released after this date)</summary>
  public DateTime? ReleaseDateFrom { get; set; }

  /// <summary>Filter by release date (games released before this date)</summary>
  public DateTime? ReleaseDateTo { get; set; }

  // Sorting
  /// <summary>Field to sort by (Title, Price, ReleaseDate, Genre)</summary>
  public string? SortBy { get; set; }

  /// <summary>Sort direction (asc or desc)</summary>
  public string SortOrder { get; set; } = "asc";

  // Validation
  /// <summary>Ensures page size doesn't exceed maximum allowed</summary>
  public void ValidateAndAdjust()
  {
    if (PageNumber < 1) PageNumber = 1;
    if (PageSize < 1) PageSize = 10;
    if (PageSize > 100) PageSize = 100;
  }
}
