using SteamClone.Backend.Entities;
using SteamClone.Backend.DTOs.Review;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SteamClone.Backend.Services;

public class ReviewService : IReviewService
{
    private readonly BackendDbContext _dbContext;
    private readonly IMapper _mapper;

    public ReviewService(BackendDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public IEnumerable<ReviewDto> GetReviewForGame(int gameId)
    {
        var gameReviews = _dbContext.Reviews
            .Where(r => r.GameId == gameId)
            .ToList();
        return _mapper.Map<IEnumerable<ReviewDto>>(gameReviews);
    }

    public ReviewDto? GetReviewById(int reviewId)
    {
        var review = _dbContext.Reviews.Find(reviewId);
        return review == null ? null : _mapper.Map<ReviewDto>(review);
    }

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

    public bool DeleteReview(int reviewId, int currentUserId, string currentUserRole)
    {
        var review = _dbContext.Reviews.Find(reviewId);
        if (review == null)
        {
            return false;
        }
        if (review.UserId != currentUserId && currentUserRole != "Admin")
        {
            return false;
        }

        _dbContext.Reviews.Remove(review);
        _dbContext.SaveChanges();

        return true;
    }

    public ReviewDto? UpdateReview(int reviewId, int currentUserId, string? comment, int? rating)
    {
        var review = _dbContext.Reviews.Find(reviewId);
        if (review == null || review.UserId != currentUserId)
        {
            return null;
        }

        if (comment != null)
        {
            review.Comment = comment;
        }
        if (rating.HasValue && rating.Value >= 1 && rating.Value <= 5)
        {
            review.Rating = rating.Value;
        }

        _dbContext.SaveChanges();

        return _mapper.Map<ReviewDto>(review);
    }
}
