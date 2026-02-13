using ToptalFinialSolution.Application.DTOs;
using ToptalFinialSolution.Application.Interfaces;
using ToptalFinialSolution.Domain.Entities;
using ToptalFinialSolution.Domain.Enums;
using ToptalFinialSolution.Domain.Interfaces;

namespace ToptalFinialSolution.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<ReviewDto>> GetReviewsAsync(ReviewListQuery query)
    {
        // Validate pagination
        if (query.Page < 1) query.Page = 1;
        if (query.PageSize < 1 || query.PageSize > 100) query.PageSize = 10;

        var (reviews, totalCount) = await _unitOfWork.Reviews.GetPagedAsync(
            query.Page,
            query.PageSize,
            query.RestaurantId
        );

        var reviewDtos = reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            ReviewText = r.ReviewText,
            Rating = r.Rating,
            RestaurantId = r.RestaurantId,
            RestaurantTitle = r.Restaurant.Title,
            ReviewerId = r.ReviewerId,
            ReviewerName = r.Reviewer.FullName,
            CreatedAt = r.CreatedAt
        });

        return new PagedResult<ReviewDto>
        {
            Items = reviewDtos,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ReviewDto> CreateReviewAsync(CreateReviewRequest request, Guid reviewerId)
    {
        // Verify reviewer exists and is of type Reviewer
        var reviewer = await _unitOfWork.Users.GetByIdAsync(reviewerId);
        if (reviewer == null || reviewer.UserType != UserType.Reviewer)
        {
            throw new UnauthorizedAccessException("Only reviewers can create reviews");
        }

        // Verify restaurant exists
        var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(request.RestaurantId);
        if (restaurant == null)
        {
            throw new KeyNotFoundException("Restaurant not found");
        }

        // Check if user already reviewed this restaurant
        var existingReview = await _unitOfWork.Reviews.FindAsync(
            r => r.RestaurantId == request.RestaurantId && r.ReviewerId == reviewerId
        );
        if (existingReview.Any())
        {
            throw new InvalidOperationException("You have already reviewed this restaurant");
        }

        // Create review
        var review = new Review
        {
            Id = Guid.NewGuid(),
            ReviewText = request.ReviewText,
            Rating = request.Rating,
            RestaurantId = request.RestaurantId,
            ReviewerId = reviewerId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Reviews.AddAsync(review);

        // Update restaurant average rating and review count
        var averageRating = await _unitOfWork.Reviews.GetAverageRatingByRestaurantAsync(request.RestaurantId);
        var reviewCount = await _unitOfWork.Reviews.GetReviewCountByRestaurantAsync(request.RestaurantId);
        
        // Add 1 to count since we're adding a new review
        restaurant.AverageRating = (averageRating * reviewCount + request.Rating) / (reviewCount + 1);
        restaurant.ReviewCount = reviewCount + 1;
        
        await _unitOfWork.Restaurants.UpdateAsync(restaurant);
        await _unitOfWork.SaveChangesAsync();

        return new ReviewDto
        {
            Id = review.Id,
            ReviewText = review.ReviewText,
            Rating = review.Rating,
            RestaurantId = review.RestaurantId,
            RestaurantTitle = restaurant.Title,
            ReviewerId = reviewerId,
            ReviewerName = reviewer.FullName,
            CreatedAt = review.CreatedAt
        };
    }
}
