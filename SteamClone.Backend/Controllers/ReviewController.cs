using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamClone.Backend.DTOs.Review;
using SteamClone.Backend.Services;
using AutoMapper;

namespace SteamClone.Backend.Controllers;

/// <summary>
/// Manages game review operations including creation, retrieval, updates, and deletion
/// </summary>
[ApiController]
[Route("store/[controller]")]
[Authorize(Roles = "Player,Admin")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes the review controller with required services
    /// </summary>
    public ReviewController(IReviewService reviewService, IUserService userService, IMapper mapper)
    {
        _reviewService = reviewService;
        _userService = userService;
        _mapper = mapper;
    }

    /// <summary>
    /// Extracts the authenticated user's ID from JWT claims
    /// </summary>
    /// <returns>User ID from the authentication token</returns>
    /// <exception cref="Exception">Thrown if User ID claim is missing</exception>
    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User ID Claim missing"));
    }

    /// <summary>
    /// Extracts the authenticated user's role from JWT claims
    /// </summary>
    /// <returns>User role from the authentication token</returns>
    /// <exception cref="Exception">Thrown if User Role claim is missing</exception>
    private string GetCurrentUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? throw new Exception("User Role Claim missing");
    }

    /// <summary>
    /// Retrieves all reviews for a specific game (accessible to all authenticated users)
    /// </summary>
    /// <param name="gameId">Game identifier</param>
    /// <returns>Collection of reviews for the specified game</returns>
    [Authorize(Roles = "Admin, Player, Publisher")]
    [HttpGet("game/{gameId}")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviewForGame(int gameId)
    {
        var gameReviews = await _reviewService.GetReviewForGameAsync(gameId);
        return Ok(gameReviews);
    }

    /// <summary>
    /// Retrieves a specific review by its ID
    /// </summary>
    /// <param name="reviewId">Review identifier</param>
    /// <returns>Review details if found, otherwise NotFound</returns>
    [HttpGet("{reviewId}")]
    public async Task<ActionResult<ReviewDto>> GetReviewById(int reviewId)
    {
        var review = await _reviewService.GetReviewByIdAsync(reviewId);
        if (review == null)
        {
            return NotFound();
        }
        return Ok(review);
    }

    /// <summary>
    /// Creates a new review for a game (Player only)
    /// </summary>
    /// <param name="gameId">Game identifier to review</param>
    /// <param name="newReviewDto">Review content including rating and comment</param>
    /// <returns>Created review with 201 status</returns>
    [HttpPost("game/{gameId}/add")]
    [Authorize(Roles = "Player")]
    public async Task<ActionResult<ReviewDto>> CreateReview(int gameId, [FromBody] ReviewCreateDto newReviewDto)
    {
        var currentUserId = GetCurrentUserId();

        // Create review associated with current user and game
        var createdReview = await _reviewService.AddReviewAsync(gameId, currentUserId, newReviewDto);

        return StatusCode(201, createdReview);
    }

    /// <summary>
    /// Deletes a review (users can delete their own reviews, admins can delete any)
    /// </summary>
    /// <param name="reviewId">Review identifier to delete</param>
    /// <returns>NoContent if successful, NotFound if review doesn't exist, Forbid if unauthorized</returns>
    [HttpDelete("{reviewId}")]
    [Authorize]
    public async Task<ActionResult> DeleteReview(int reviewId)
    {
        var review = await _reviewService.GetReviewByIdAsync(reviewId);
        if (review == null)
        {
            return NotFound();
        }

        var currentUserId = GetCurrentUserId();
        var currentUserRole = GetCurrentUserRole();

        // Verify user owns the review or is an admin
        var success = await _reviewService.DeleteReviewAsync(reviewId, currentUserId, currentUserRole);
        if (!success)
        {
            return Forbid();
        }
        return NoContent();
    }

    /// <summary>
    /// Updates an existing review (Player can only update their own reviews)
    /// </summary>
    /// <param name="reviewId">Review identifier to update</param>
    /// <param name="updatedReviewDto">Updated review content</param>
    /// <returns>Updated review details if found and authorized, otherwise NotFound</returns>
    [HttpPatch("{reviewId}/update")]
    [Authorize(Roles = "Player")]
    public async Task<ActionResult<ReviewDto>> UpdateReview(int reviewId, [FromBody] ReviewCreateDto updatedReviewDto)
    {
        var currentUserId = GetCurrentUserId();

        // Update review only if user owns it
        var updatedReview = await _reviewService.UpdateReviewAsync(reviewId, currentUserId, updatedReviewDto.Comment, updatedReviewDto.Rating);
        if (updatedReview == null)
        {
            return NotFound();
        }
        return Ok(updatedReview);
    }
}