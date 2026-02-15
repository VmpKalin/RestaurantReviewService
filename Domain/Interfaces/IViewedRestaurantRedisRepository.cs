namespace ToptalFinialSolution.Domain.Interfaces;

/// <summary>
/// Redis repository for storing viewed restaurants history per user
/// Key: UserId, Value: RestaurantId, Score: Unix timestamp of view
/// </summary>
public interface IViewedRestaurantRedisRepository : IRedisRepository<Guid, Guid>
{
    /// <summary>
    /// Records a restaurant view for a user with the current timestamp
    /// </summary>
    Task RecordViewAsync(Guid userId, Guid restaurantId);
    
    /// <summary>
    /// Gets the last N viewed restaurant IDs for a user
    /// </summary>
    Task<List<Guid>> GetRecentlyViewedRestaurantIdsAsync(Guid userId, int count = 10);
}
