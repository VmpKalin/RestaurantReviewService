using StackExchange.Redis;
using ToptalFinialSolution.Domain.Interfaces;

namespace ToptalFinialSolution.Infrastructure.Repositories.Base.Cache;

/// <summary>
/// Base Redis repository implementation using Sorted Sets for time-based ordering.
/// Key format follows: domain:subdomain:resource:{id}
/// </summary>
public abstract class RedisRepository<TKey, TValue>(
    IConnectionMultiplexer redis,
    string keyPrefix,
    int maxEntries = 100,
    int expirationDays = 90)
    : IRedisRepository<TKey, TValue>
{
    protected readonly IDatabase Db = redis.GetDatabase();
    protected readonly string KeyPrefix = keyPrefix;
    protected readonly int MaxEntries = maxEntries;
    protected readonly TimeSpan Expiration = TimeSpan.FromDays(expirationDays);

    public virtual async Task AddAsync(TKey key, TValue value, CancellationToken cancellationToken = default)
    {
        var score = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        await AddAsync(key, value, score, cancellationToken);
    }

    public virtual async Task AddAsync(TKey key, TValue value, double score, CancellationToken cancellationToken = default)
    {
        var redisKey = GetRedisKey(key);
        var redisValue = SerializeValue(value);

        await Db.SortedSetAddAsync(redisKey, redisValue, score);
        await Db.SortedSetRemoveRangeByRankAsync(redisKey, 0, -MaxEntries - 1);
        await Db.KeyExpireAsync(redisKey, Expiration);
    }

    public virtual async Task<IReadOnlyList<TValue>> GetRecentAsync(TKey key, int count, CancellationToken cancellationToken = default)
    {
        var redisKey = GetRedisKey(key);
        var entries = await Db.SortedSetRangeByRankAsync(redisKey, -count, -1, Order.Descending);

        return entries
            .Select(entry => DeserializeValue(entry))
            .ToList();
    }

    public virtual async Task RemoveAsync(TKey key, TValue value, CancellationToken cancellationToken = default)
    {
        var redisKey = GetRedisKey(key);
        var redisValue = SerializeValue(value);
        await Db.SortedSetRemoveAsync(redisKey, redisValue);
    }

    public virtual async Task ClearAsync(TKey key, CancellationToken cancellationToken = default)
    {
        var redisKey = GetRedisKey(key);
        await Db.KeyDeleteAsync(redisKey);
    }

    public virtual async Task<long> CountAsync(TKey key, CancellationToken cancellationToken = default)
    {
        var redisKey = GetRedisKey(key);
        return await Db.SortedSetLengthAsync(redisKey);
    }

    public virtual async Task<bool> ExistsAsync(TKey key, TValue value, CancellationToken cancellationToken = default)
    {
        var redisKey = GetRedisKey(key);
        var redisValue = SerializeValue(value);
        var score = await Db.SortedSetScoreAsync(redisKey, redisValue);
        return score.HasValue;
    }

    /// <summary>
    /// Constructs the full Redis key: {prefix}{key}
    /// </summary>
    protected virtual string GetRedisKey(TKey key) => $"{KeyPrefix}{key}";

    protected abstract string SerializeValue(TValue value);
    protected abstract TValue DeserializeValue(RedisValue redisValue);
}
