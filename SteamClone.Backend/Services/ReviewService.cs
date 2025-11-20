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
    public IEnumerable<ReviewDto> GetReviewForGame(int gameId)
    {
        var gameReviews = _dbContext.Reviews
            .Where(r => r.GameId == gameId)
            .ToList();
        return _mapper.Map<IEnumerable<ReviewDto>>(gameReviews);
    }

    /// <summary>
    /// Retrieves a specific review by its identifier
    /// </summary>
    /// <param name="reviewId">Review ID</param>
    /// <returns>Review DTO if found, null otherwise</returns>
    public ReviewDto? GetReviewById(int reviewId)
    {
        var review = _dbContext.Reviews.Find(reviewId);
        return review == null ? null : _mapper.Map<ReviewDto>(review);
    }

    /// <summary>
    /// Creates a new review for a game
    /// </summary>
    /// <param name="gameId">Game ID being reviewed</param>
    /// <param name="userId">User ID creating the review</param>
    /// <param name="newReviewDto">Review content including rating and comment</param>
    /// <returns>Created review DTO with generated ID</returns>
    public ReviewDto AddReview(int gameId, int userId, ReviewCreateDto newReviewDto)
    {
        var newReview = _mapper.Map<Reviews>(newReviewDto);
        newReview.GameId = gameId;
        newReview.UserId = userId;
        newReview.ReviewDate = DateTime.UtcNow;

        _dbContext.Reviews.Add(newReview);
        _dbContext.SaveChanges();

        return _mapper.Map<ReviewDto>(newReview);
    }

    /// <summary>
    /// Deletes a review with authorization check (users can delete their own, admins can delete any)
    /// </summary>
    /// <param name="reviewId">Review ID to delete</param>
    /// <param name="currentUserId">User ID attempting deletion</param>
    /// <param name="currentUserRole">Role of user attempting deletion</param>
    /// <returns>True if deletion successful, false if not found or unauthorized</returns>
    public bool DeleteReview(int reviewId, int currentUserId, string currentUserRole)
    {
        var review = _dbContext.Reviews.Find(reviewId);
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
        _dbContext.SaveChanges();

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
    public ReviewDto? UpdateReview(int reviewId, int currentUserId, string? comment, int? rating)
    {
        var review = _dbContext.Reviews.Find(reviewId);

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

        _dbContext.SaveChanges();

        return _mapper.Map<ReviewDto>(review);
    }
}
