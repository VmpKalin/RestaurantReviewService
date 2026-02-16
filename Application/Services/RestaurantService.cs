using NetTopologySuite.Geometries;
using ToptalFinialSolution.Application.DTOs;
using ToptalFinialSolution.Application.Interfaces;
using ToptalFinialSolution.Domain.Entities;
using ToptalFinialSolution.Domain.Enums;
using ToptalFinialSolution.Domain.Exceptions;
using ToptalFinialSolution.Domain.Interfaces;

namespace ToptalFinialSolution.Application.Services;

public class RestaurantService(IUnitOfWork unitOfWork, IGeocodingService geocodingService) : IRestaurantService
{
    public async Task<PagedResult<RestaurantDto>> GetRestaurantsAsync(RestaurantListQuery query, CancellationToken cancellationToken = default)
    {
        var page = query.Page is < 1 ? 1 : query.Page;
        var pageSize = query.PageSize is < 1 or > 100 ? 10 : query.PageSize;

        var (restaurants, totalCount) = await unitOfWork.Restaurants.GetPagedAsync(
            page,
            pageSize,
            query.TitleFilter,
            query.Latitude,
            query.Longitude,
            query.RadiusKm,
            cancellationToken
        );

        Point? searchPoint = null;
        if (query is { Latitude: not null, Longitude: not null, RadiusKm: not null })
        {
            searchPoint = new Point(query.Longitude.Value, query.Latitude.Value) { SRID = 4326 };
        }

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
            CreatedAt = r.CreatedAt,
            DistanceKm = searchPoint is not null && r.Location is not null
                ? Math.Round(CalculateDistanceKm(query.Latitude!.Value, query.Longitude!.Value, r.Latitude, r.Longitude), 2)
                : null
        }).ToList();

        return new PagedResult<RestaurantDto>
        {
            Items = restaurantDtos,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371.0;
        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;

    public async Task<RestaurantDto?> GetRestaurantByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var restaurant = await unitOfWork.Restaurants.GetByIdWithReviewsAsync(id, cancellationToken);
        if (restaurant is null) return null;

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

    public async Task<RestaurantDto> CreateRestaurantAsync(CreateRestaurantRequest request, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var owner = await unitOfWork.Users.GetByIdAsync(ownerId, cancellationToken);
        if (owner is not { UserType: UserType.Owner })
        {
            throw new ForbiddenException("Only owners can create restaurants");
        }

        double latitude, longitude;

        if (request is { Latitude: not null, Longitude: not null })
        {
            latitude = request.Latitude.Value;
            longitude = request.Longitude.Value;
        }
        else if (!string.IsNullOrWhiteSpace(request.Address))
        {
            var coordinates = await geocodingService.GeocodeAddressAsync(request.Address, cancellationToken);
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
            CreatedAt = DateTimeOffset.UtcNow,
            AverageRating = 0,
            ReviewCount = 0
        };
        restaurant.SetCoordinates(latitude, longitude);

        await unitOfWork.Restaurants.AddAsync(restaurant, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

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

    public async Task<RestaurantDto> UpdateRestaurantAsync(Guid id, UpdateRestaurantRequest request, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var restaurant = await unitOfWork.Restaurants.GetByIdAsync(id, cancellationToken);
        if (restaurant is null)
        {
            throw new KeyNotFoundException("Restaurant not found");
        }

        if (restaurant.OwnerId != ownerId)
        {
            throw new ForbiddenException("You can only update your own restaurants");
        }

        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            restaurant.Title = request.Title;
        }

        if (request.PreviewImage is not null)
        {
            restaurant.PreviewImage = request.PreviewImage;
        }

        if (!string.IsNullOrWhiteSpace(request.Description))
        {
            restaurant.Description = request.Description;
        }

        if (request is { Latitude: not null, Longitude: not null })
        {
            restaurant.SetCoordinates(request.Latitude.Value, request.Longitude.Value);
        }
        else if (!string.IsNullOrWhiteSpace(request.Address))
        {
            var coordinates = await geocodingService.GeocodeAddressAsync(request.Address, cancellationToken);
            if (!coordinates.HasValue)
            {
                throw new InvalidOperationException("Unable to geocode the provided address");
            }
            restaurant.SetCoordinates(coordinates.Value.Latitude, coordinates.Value.Longitude);
        }

        restaurant.UpdatedAt = DateTimeOffset.UtcNow;

        await unitOfWork.Restaurants.UpdateAsync(restaurant);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var owner = await unitOfWork.Users.GetByIdAsync(ownerId, cancellationToken);

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

    public async Task DeleteRestaurantAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {
        var restaurant = await unitOfWork.Restaurants.GetByIdAsync(id, cancellationToken);
        if (restaurant is null)
        {
            throw new KeyNotFoundException("Restaurant not found");
        }

        if (restaurant.OwnerId != ownerId)
        {
            throw new ForbiddenException("You can only delete your own restaurants");
        }

        await unitOfWork.Restaurants.DeleteAsync(restaurant);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
