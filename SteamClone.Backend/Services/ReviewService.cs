using SteamClone.Backend.Entities;
using SteamClone.Backend.DTOs.Review;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SteamClone.Backend.Services;

/// <summary>
/// Service for managing game reviews including CRUD operations and authorization checks
/// </summary>
public class ReviewService : IReviewService
{
    private readonly BackendDbContext _dbContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes the review service with database context and AutoMapper
    /// </summary>
    public ReviewService(BackendDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all reviews for a specific game
    /// </summary>
    /// <param name="gameId">Game ID to get reviews for</param>
    /// <returns>Collection of review DTOs for the specified game</returns>
    public async Task<IEnumerable<ReviewDto>> GetReviewForGameAsync(int gameId)
    {
        var gameReviews = await _dbContext.Reviews
            .Include(r => r.User)
            .Include(r => r.Game)
            .Where(r => r.GameId == gameId)
            .ToListAsync();
        return _mapper.Map<IEnumerable<ReviewDto>>(gameReviews);
    }

    /// <summary>
    /// Retrieves a specific review by its identifier
    /// </summary>
    /// <param name="reviewId">Review ID</param>
    /// <returns>Review DTO if found, null otherwise</returns>
    public async Task<ReviewDto?> GetReviewByIdAsync(int reviewId)
    {
        var review = await _dbContext.Reviews
            .Include(r => r.User)
            .Include(r => r.Game)
            .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        return review == null ? null : _mapper.Map<ReviewDto>(review);
    }

    /// <summary>
    /// Creates a new review for a game
    /// </summary>
    /// <param name="gameId">Game ID being reviewed</param>
    /// <param name="userId">User ID creating the review</param>
    /// <param name="newReviewDto">Review content including rating and comment</param>
    /// <returns>Created review DTO with generated ID</returns>
    public async Task<ReviewDto> AddReviewAsync(int gameId, int userId, ReviewCreateDto newReviewDto)
    {
        var newReview = _mapper.Map<Review>(newReviewDto);
        newReview.GameId = gameId;
        newReview.UserId = userId;
        newReview.ReviewDate = DateTime.UtcNow;

        _dbContext.Reviews.Add(newReview);
        await _dbContext.SaveChangesAsync();

        // Reload review with navigation properties
        var createdReview = await _dbContext.Reviews
            .Include(r => r.User)
            .Include(r => r.Game)
            .FirstOrDefaultAsync(r => r.ReviewId == newReview.ReviewId);

        return _mapper.Map<ReviewDto>(createdReview);
    }

    /// <summary>
    /// Deletes a review with authorization check (users can delete their own, admins can delete any)
    /// </summary>
    /// <param name="reviewId">Review ID to delete</param>
    /// <param name="currentUserId">User ID attempting deletion</param>
    /// <param name="currentUserRole">Role of user attempting deletion</param>
    /// <returns>True if deletion successful, false if not found or unauthorized</returns>
    public async Task<bool> DeleteReviewAsync(int reviewId, int currentUserId, string currentUserRole)
    {
        var review = await _dbContext.Reviews.FindAsync(reviewId);
        if (review == null)
        {
            return false;
        }

        // Verify user owns the review or is an admin
        if (review.UserId != currentUserId && currentUserRole != "Admin")
        {
            return false;
        }

        _dbContext.Reviews.Remove(review);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Updates an existing review (users can only update their own reviews)
    /// </summary>
    /// <param name="reviewId">Review ID to update</param>
    /// <param name="currentUserId">User ID attempting the update</param>
    /// <param name="comment">Updated comment text (optional)</param>
    /// <param name="rating">Updated rating (optional, must be 1-5)</param>
    /// <returns>Updated review DTO if successful, null if not found or unauthorized</returns>
    public async Task<ReviewDto?> UpdateReviewAsync(int reviewId, int currentUserId, string? comment, int? rating)
    {
        var review = await _dbContext.Reviews.FindAsync(reviewId);

        // Verify review exists and user owns it
        if (review == null || review.UserId != currentUserId)
        {
            return null;
        }

        // Update comment if provided
        if (comment != null)
        {
            review.Comment = comment;
        }

        // Update rating if provided and valid (1-5 range)
        if (rating.HasValue && rating.Value >= 1 && rating.Value <= 5)
        {
            review.Rating = rating.Value;
        }

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<ReviewDto>(review);
    }
}
