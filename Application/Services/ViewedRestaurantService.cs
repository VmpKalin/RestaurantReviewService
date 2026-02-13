using ToptalFinialSolution.Application.DTOs;
using ToptalFinialSolution.Application.Interfaces;
using ToptalFinialSolution.Domain.Enums;
using ToptalFinialSolution.Domain.Interfaces;

namespace ToptalFinialSolution.Application.Services;

public class ViewedRestaurantService : IViewedRestaurantService
{
    private readonly IUnitOfWork _unitOfWork;

    public ViewedRestaurantService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task RecordViewAsync(Guid userId, Guid restaurantId)
    {
        // Verify user is a reviewer
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null || user.UserType != UserType.Reviewer)
        {
            throw new UnauthorizedAccessException("Only reviewers can record restaurant views");
        }

        // Verify restaurant exists
        var restaurant = await _unitOfWork.Restaurants.GetByIdAsync(restaurantId);
        if (restaurant == null)
        {
            throw new KeyNotFoundException("Restaurant not found");
        }

        await _unitOfWork.ViewedRestaurants.RecordViewAsync(userId, restaurantId);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<RestaurantDto>> GetRecentlyViewedAsync(Guid userId)
    {
        // Verify user is a reviewer
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null || user.UserType != UserType.Reviewer)
        {
            throw new UnauthorizedAccessException("Only reviewers can view recently viewed restaurants");
        }

        var restaurants = await _unitOfWork.ViewedRestaurants.GetRecentlyViewedAsync(userId, 10);

        return restaurants.Select(r => new RestaurantDto
        {
            Id = r.Id,
            Title = r.Title,
            PreviewImage = r.PreviewImage,
            Latitude = r.Latitude,
            Longitude = r.Longitude,
            Description = r.Description,
            AverageRating = r.AverageRating,
            ReviewCount = r.ReviewCount,
            OwnerId = r.OwnerId,
            OwnerName = r.Owner.FullName,
            CreatedAt = r.CreatedAt
        });
    }
}
