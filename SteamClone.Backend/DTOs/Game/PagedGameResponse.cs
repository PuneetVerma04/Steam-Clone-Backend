namespace SteamClone.Backend.DTOs.Game;

/// <summary>
/// Paginated response for game queries with metadata
/// </summary>
public class PagedGameResponse
{
  /// <summary>Collection of games for the current page</summary>
  public IEnumerable<GameResponseDTO> Games { get; set; } = new List<GameResponseDTO>();

  /// <summary>Current page number</summary>
  public int PageNumber { get; set; }

  /// <summary>Number of items per page</summary>
  public int PageSize { get; set; }

  /// <summary>Total number of items matching the query</summary>
  public int TotalCount { get; set; }

  /// <summary>Total number of pages</summary>
  public int TotalPages { get; set; }

  /// <summary>Indicates if there is a previous page</summary>
  public bool HasPrevious => PageNumber > 1;

  /// <summary>Indicates if there is a next page</summary>
  public bool HasNext => PageNumber < TotalPages;
}
