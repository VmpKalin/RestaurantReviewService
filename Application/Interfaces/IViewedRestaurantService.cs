using ToptalFinialSolution.Application.DTOs;

namespace ToptalFinialSolution.Application.Interfaces;

public interface IViewedRestaurantService
{
    Task RecordViewAsync(Guid userId, Guid restaurantId);
    Task<IEnumerable<RestaurantDto>> GetRecentlyViewedAsync(Guid userId);
}
