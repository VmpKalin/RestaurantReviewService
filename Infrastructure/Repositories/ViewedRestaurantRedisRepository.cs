using StackExchange.Redis;
using ToptalFinialSolution.Domain.Interfaces;

namespace ToptalFinialSolution.Infrastructure.Repositories;

/// <summary>
/// Redis repository implementation for storing viewed restaurants history
/// Uses Sorted Sets with UserId as key, RestaurantId as member, and Unix timestamp as score
/// </summary>
public class ViewedRestaurantRedisRepository(IConnectionMultiplexer redis)
    : RedisRepository<Guid, Guid>(redis, KeyPrefix, MaxViewedRestaurants, ExpirationDays),
        IViewedRestaurantRedisRepository
{
    private const string KeyPrefix = "viewed:restaurants:";
    private const int MaxViewedRestaurants = 100;
    private const int ExpirationDays = 90;

    public async Task RecordViewAsync(Guid userId, Guid restaurantId)
    {
        await AddAsync(userId, restaurantId);
    }

    public async Task<List<Guid>> GetRecentlyViewedRestaurantIdsAsync(Guid userId, int count = 10)
    {
        return await GetRecentAsync(userId, count);
    }

    protected override string SerializeValue(Guid value)
    {
        return value.ToString();
    }

    protected override Guid DeserializeValue(RedisValue redisValue)
    {
        return Guid.Parse(redisValue.ToString());
    }
}
