using ToptalFinialSolution.Application.DTOs;
using ToptalFinialSolution.Application.Interfaces;
using ToptalFinialSolution.Domain.Entities;
using ToptalFinialSolution.Domain.Enums;
using ToptalFinialSolution.Domain.Exceptions;
using ToptalFinialSolution.Domain.Interfaces;

namespace ToptalFinialSolution.Application.Services;

public class ReviewService(IUnitOfWork unitOfWork) : IReviewService
{
    public async Task<PagedResult<ReviewDto>> GetReviewsAsync(ReviewListQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page is < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 or > 100 ? 10 : query.PageSize;

        var (reviews, totalCount) = await unitOfWork.Reviews.GetPagedAsync(
            page,
            pageSize,
            query.RestaurantId,
            cancellationToken
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
        }).ToList();

        return new PagedResult<ReviewDto>
        {
            Items = reviewDtos,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<ReviewDto> CreateReviewAsync(CreateReviewRequest request, Guid reviewerId, CancellationToken cancellationToken = default)
    {
        var reviewer = await unitOfWork.Users.GetByIdAsync(reviewerId, cancellationToken);
        if (reviewer is not { UserType: UserType.Reviewer })
        {
            throw new ForbiddenException("Only reviewers can create reviews");
        }

        var restaurant = await unitOfWork.Restaurants.GetByIdAsync(request.RestaurantId, cancellationToken);
        if (restaurant is null)
        {
            throw new KeyNotFoundException("Restaurant not found");
        }

        var existingReview = await unitOfWork.Reviews.FindAsync(
            r => r.RestaurantId == request.RestaurantId && r.ReviewerId == reviewerId,
            cancellationToken
        );
        if (existingReview.Count is > 0)
        {
            throw new InvalidOperationException("You have already reviewed this restaurant");
        }

        var review = new Review
        {
            Id = Guid.NewGuid(),
            ReviewText = request.ReviewText,
            Rating = request.Rating,
            RestaurantId = request.RestaurantId,
            ReviewerId = reviewerId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await unitOfWork.Reviews.AddAsync(review, cancellationToken);

        var averageRating = await unitOfWork.Reviews.GetAverageRatingByRestaurantAsync(request.RestaurantId, cancellationToken);
        var reviewCount = await unitOfWork.Reviews.GetReviewCountByRestaurantAsync(request.RestaurantId, cancellationToken);

        restaurant.AverageRating = (averageRating * reviewCount + request.Rating) / (reviewCount + 1);
        restaurant.ReviewCount = reviewCount + 1;

        await unitOfWork.Restaurants.UpdateAsync(restaurant);
        await unitOfWork.SaveChangesAsync(cancellationToken);

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
