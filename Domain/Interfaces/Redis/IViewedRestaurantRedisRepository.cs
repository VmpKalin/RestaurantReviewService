namespace ToptalFinialSolution.Domain.Interfaces;

/// <summary>
/// Redis repository for storing viewed restaurants history per user.
/// Key pattern: restaurants:views:user:{userId}
/// </summary>
public interface IViewedRestaurantRedisRepository : IRedisRepository<Guid, Guid>
{
    Task RecordViewAsync(Guid userId, Guid restaurantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Guid>> GetRecentlyViewedRestaurantIdsAsync(Guid userId, int count = 10, CancellationToken cancellationToken = default);
}
