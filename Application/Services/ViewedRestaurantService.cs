using Microsoft.EntityFrameworkCore;
using ToptalFinialSolution.Application.DTOs;
using ToptalFinialSolution.Application.Interfaces;
using ToptalFinialSolution.Domain.Enums;
using ToptalFinialSolution.Domain.Exceptions;
using ToptalFinialSolution.Domain.Interfaces;
using ToptalFinialSolution.Infrastructure.Data;

namespace ToptalFinialSolution.Application.Services;

public class ViewedRestaurantService(
    IUnitOfWork unitOfWork,
    IViewedRestaurantRedisRepository redisRepository,
    ApplicationDbContext context)
    : IViewedRestaurantService
{
    public async Task RecordViewAsync(Guid userId, Guid restaurantId, CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user is not { UserType: UserType.Reviewer })
        {
            throw new ForbiddenException("Only reviewers can record restaurant views");
        }

        var restaurant = await unitOfWork.Restaurants.GetByIdAsync(restaurantId, cancellationToken);
        if (restaurant is null)
        {
            throw new KeyNotFoundException("Restaurant not found");
        }

        await redisRepository.RecordViewAsync(userId, restaurantId, cancellationToken);
    }

    public async Task<IReadOnlyList<RestaurantDto>> GetRecentlyViewedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user is not { UserType: UserType.Reviewer })
        {
            throw new ForbiddenException("Only reviewers can view recently viewed restaurants");
        }

        var restaurantIds = await redisRepository.GetRecentlyViewedRestaurantIdsAsync(userId, 10, cancellationToken);

        if (restaurantIds.Count is 0)
        {
            return [];
        }

        var restaurants = await context.Restaurants
            .Where(r => restaurantIds.Contains(r.Id))
            .Include(r => r.Owner)
            .ToListAsync(cancellationToken);

        return restaurants
            .Select(r => new RestaurantDto
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
            })
            .ToList();
    }
}
