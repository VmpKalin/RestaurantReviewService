using Microsoft.EntityFrameworkCore;
using ToptalFinialSolution.Application.DTOs;
using ToptalFinialSolution.Application.Interfaces;
using ToptalFinialSolution.Domain.Enums;
using ToptalFinialSolution.Domain.Interfaces;
using ToptalFinialSolution.Infrastructure.Data;

namespace ToptalFinialSolution.Application.Services;

public class ViewedRestaurantService : IViewedRestaurantService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IViewedRestaurantRedisRepository _redisRepository;
    private readonly ApplicationDbContext _context;

    public ViewedRestaurantService(
        IUnitOfWork unitOfWork, 
        IViewedRestaurantRedisRepository redisRepository,
        ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _redisRepository = redisRepository;
        _context = context;
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

        // Record view in Redis (no DB persistence needed)
        await _redisRepository.RecordViewAsync(userId, restaurantId);
    }

    public async Task<IEnumerable<RestaurantDto>> GetRecentlyViewedAsync(Guid userId)
    {
        // Verify user is a reviewer
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null || user.UserType != UserType.Reviewer)
        {
            throw new UnauthorizedAccessException("Only reviewers can view recently viewed restaurants");
        }

        // Get restaurant IDs from Redis
        var restaurantIds = await _redisRepository.GetRecentlyViewedRestaurantIdsAsync(userId, 10);
        
        if (!restaurantIds.Any())
        {
            return Enumerable.Empty<RestaurantDto>();
        }

        // Fetch restaurant details from database
        var restaurants = await _context.Restaurants
            .Where(r => restaurantIds.Contains(r.Id))
            .Include(r => r.Owner)
            .ToListAsync();
        
        // Maintain the order from Redis (most recent first)
        var orderedRestaurants = restaurantIds
            .Select(id => restaurants.FirstOrDefault(r => r.Id == id))
            .Where(r => r != null)
            .ToList();

        return orderedRestaurants.Select(r => new RestaurantDto
        {
            Id = r!.Id,
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
