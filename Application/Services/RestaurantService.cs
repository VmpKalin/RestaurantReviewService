using ToptalFinialSolution.Application.DTOs;
using ToptalFinialSolution.Application.Interfaces;
using ToptalFinialSolution.Domain.Entities;
using ToptalFinialSolution.Domain.Enums;
using ToptalFinialSolution.Domain.Interfaces;

namespace ToptalFinialSolution.Application.Services;

public class RestaurantService(IUnitOfWork unitOfWork, IGeocodingService geocodingService) : IRestaurantService
{
    public async Task<PagedResult<RestaurantDto>> GetRestaurantsAsync(RestaurantListQuery query)
    {
        // Validate pagination
        if (query.Page < 1) query.Page = 1;
        if (query.PageSize < 1 || query.PageSize > 100) query.PageSize = 10;

        var (restaurants, totalCount) = await unitOfWork.Restaurants.GetPagedAsync(
            query.Page,
            query.PageSize,
            query.TitleFilter,
            query.Latitude,
            query.Longitude,
            query.RadiusInMiles
        );

        var restaurantDtos = restaurants.Select(r => new RestaurantDto
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

        return new PagedResult<RestaurantDto>
        {
            Items = restaurantDtos,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<RestaurantDto?> GetRestaurantByIdAsync(Guid id)
    {
        var restaurant = await unitOfWork.Restaurants.GetByIdWithReviewsAsync(id);
        if (restaurant == null) return null;

        return new RestaurantDto
        {
            Id = restaurant.Id,
            Title = restaurant.Title,
            PreviewImage = restaurant.PreviewImage,
            Latitude = restaurant.Latitude,
            Longitude = restaurant.Longitude,
            Description = restaurant.Description,
            AverageRating = restaurant.AverageRating,
            ReviewCount = restaurant.ReviewCount,
            OwnerId = restaurant.OwnerId,
            OwnerName = restaurant.Owner.FullName,
            CreatedAt = restaurant.CreatedAt
        };
    }

    public async Task<RestaurantDto> CreateRestaurantAsync(CreateRestaurantRequest request, Guid ownerId)
    {
        // Verify owner exists and is of type Owner
        var owner = await unitOfWork.Users.GetByIdAsync(ownerId);
        if (owner == null || owner.UserType != UserType.Owner)
        {
            throw new UnauthorizedAccessException("Only owners can create restaurants");
        }

        double latitude, longitude;

        // Determine coordinates
        if (request.Latitude.HasValue && request.Longitude.HasValue)
        {
            latitude = request.Latitude.Value;
            longitude = request.Longitude.Value;
        }
        else if (!string.IsNullOrWhiteSpace(request.Address))
        {
            var coordinates = await geocodingService.GeocodeAddressAsync(request.Address);
            if (!coordinates.HasValue)
            {
                throw new InvalidOperationException("Unable to geocode the provided address");
            }
            latitude = coordinates.Value.Latitude;
            longitude = coordinates.Value.Longitude;
        }
        else
        {
            throw new InvalidOperationException("Either coordinates or address must be provided");
        }

        var restaurant = new Restaurant
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            PreviewImage = request.PreviewImage,
            Description = request.Description,
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow,
            AverageRating = 0,
            ReviewCount = 0
        };
        restaurant.SetCoordinates(latitude, longitude);

        await unitOfWork.Restaurants.AddAsync(restaurant);
        await unitOfWork.SaveChangesAsync();

        return new RestaurantDto
        {
            Id = restaurant.Id,
            Title = restaurant.Title,
            PreviewImage = restaurant.PreviewImage,
            Latitude = restaurant.Latitude,
            Longitude = restaurant.Longitude,
            Description = restaurant.Description,
            AverageRating = restaurant.AverageRating,
            ReviewCount = restaurant.ReviewCount,
            OwnerId = restaurant.OwnerId,
            OwnerName = owner.FullName,
            CreatedAt = restaurant.CreatedAt
        };
    }

    public async Task<RestaurantDto> UpdateRestaurantAsync(Guid id, UpdateRestaurantRequest request, Guid ownerId)
    {
        var restaurant = await unitOfWork.Restaurants.GetByIdAsync(id);
        if (restaurant == null)
        {
            throw new KeyNotFoundException("Restaurant not found");
        }

        // Verify ownership
        if (restaurant.OwnerId != ownerId)
        {
            throw new UnauthorizedAccessException("You can only update your own restaurants");
        }

        // Update fields if provided
        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            restaurant.Title = request.Title;
        }

        if (request.PreviewImage != null)
        {
            restaurant.PreviewImage = request.PreviewImage;
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            restaurant.Description = request.Description;
        }

        // Update coordinates (keeps Location point in sync)
        if (request.Latitude.HasValue && request.Longitude.HasValue)
        {
            restaurant.SetCoordinates(request.Latitude.Value, request.Longitude.Value);
        }
        else if (!string.IsNullOrWhiteSpace(request.Address))
        {
            var coordinates = await geocodingService.GeocodeAddressAsync(request.Address);
            if (!coordinates.HasValue)
            {
                throw new InvalidOperationException("Unable to geocode the provided address");
            }
            restaurant.SetCoordinates(coordinates.Value.Latitude, coordinates.Value.Longitude);
        }

        restaurant.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.Restaurants.UpdateAsync(restaurant);
        await unitOfWork.SaveChangesAsync();

        var owner = await unitOfWork.Users.GetByIdAsync(ownerId);

        return new RestaurantDto
        {
            Id = restaurant.Id,
            Title = restaurant.Title,
            PreviewImage = restaurant.PreviewImage,
            Latitude = restaurant.Latitude,
            Longitude = restaurant.Longitude,
            Description = restaurant.Description,
            AverageRating = restaurant.AverageRating,
            ReviewCount = restaurant.ReviewCount,
            OwnerId = restaurant.OwnerId,
            OwnerName = owner?.FullName ?? string.Empty,
            CreatedAt = restaurant.CreatedAt
        };
    }

    public async Task DeleteRestaurantAsync(Guid id, Guid ownerId)
    {
        var restaurant = await unitOfWork.Restaurants.GetByIdAsync(id);
        if (restaurant == null)
        {
            throw new KeyNotFoundException("Restaurant not found");
        }

        // Verify ownership
        if (restaurant.OwnerId != ownerId)
        {
            throw new UnauthorizedAccessException("You can only delete your own restaurants");
        }

        await unitOfWork.Restaurants.DeleteAsync(restaurant);
        await unitOfWork.SaveChangesAsync();
    }
}
