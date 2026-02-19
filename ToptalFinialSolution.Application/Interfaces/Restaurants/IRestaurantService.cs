using ToptalFinialSolution.Application.DTOs;

namespace ToptalFinialSolution.Application.Interfaces;

public interface IRestaurantService
{
    Task<PagedResult<RestaurantDto>> GetRestaurantsAsync(RestaurantListQuery query, CancellationToken cancellationToken = default);
    Task<RestaurantDto?> GetRestaurantByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RestaurantDto> CreateRestaurantAsync(CreateRestaurantRequest request, Guid ownerId, CancellationToken cancellationToken = default);
    Task<RestaurantDto> UpdateRestaurantAsync(Guid id, UpdateRestaurantRequest request, Guid ownerId, CancellationToken cancellationToken = default);
    Task DeleteRestaurantAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);
}
