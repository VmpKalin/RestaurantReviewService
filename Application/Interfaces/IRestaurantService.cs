using ToptalFinialSolution.Application.DTOs;

namespace ToptalFinialSolution.Application.Interfaces;

public interface IRestaurantService
{
    Task<PagedResult<RestaurantDto>> GetRestaurantsAsync(RestaurantListQuery query);
    Task<RestaurantDto?> GetRestaurantByIdAsync(Guid id);
    Task<RestaurantDto> CreateRestaurantAsync(CreateRestaurantRequest request, Guid ownerId);
    Task<RestaurantDto> UpdateRestaurantAsync(Guid id, UpdateRestaurantRequest request, Guid ownerId);
    Task DeleteRestaurantAsync(Guid id, Guid ownerId);
}
