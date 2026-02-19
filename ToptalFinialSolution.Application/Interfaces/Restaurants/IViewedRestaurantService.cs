using ToptalFinialSolution.Application.DTOs;

namespace ToptalFinialSolution.Application.Interfaces;

public interface IViewedRestaurantService
{
    Task RecordViewAsync(Guid userId, Guid restaurantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RestaurantDto>> GetRecentlyViewedAsync(Guid userId, CancellationToken cancellationToken = default);
}
