using StackExchange.Redis;
using ToptalFinialSolution.Domain.Interfaces;

namespace ToptalFinialSolution.Infrastructure.Repositories;

/// <summary>
/// Redis repository for storing viewed restaurants history.
/// Key pattern: restaurants:views:user:{userId}
/// </summary>
public class ViewedRestaurantRedisRepository(IConnectionMultiplexer redis)
    : RedisRepository<Guid, Guid>(redis, "restaurants:views:user:", MaxViewedRestaurants, ExpirationDays),
        IViewedRestaurantRedisRepository
{
    private const int MaxViewedRestaurants = 10;
    private const int ExpirationDays = 7;

    public async Task RecordViewAsync(Guid userId, Guid restaurantId, CancellationToken cancellationToken = default)
    {
        await AddAsync(userId, restaurantId, cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> GetRecentlyViewedRestaurantIdsAsync(Guid userId, int count = 10, CancellationToken cancellationToken = default)
    {
        return await GetRecentAsync(userId, count, cancellationToken);
    }

    protected override string SerializeValue(Guid value) => value.ToString();

    protected override Guid DeserializeValue(RedisValue redisValue) => Guid.Parse(redisValue.ToString());
}
