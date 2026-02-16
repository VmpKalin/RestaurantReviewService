using StackExchange.Redis;
using ToptalFinialSolution.Domain.Interfaces;

namespace ToptalFinialSolution.Infrastructure.Repositories;

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
    protected readonly IDatabase _db = redis.GetDatabase();
    protected readonly string _keyPrefix = keyPrefix;
    protected readonly int _maxEntries = maxEntries;
    protected readonly TimeSpan _expiration = TimeSpan.FromDays(expirationDays);

    public virtual async Task AddAsync(TKey key, TValue value, CancellationToken cancellationToken = default)
    {
        var score = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        await AddAsync(key, value, score, cancellationToken);
    }

    public virtual async Task AddAsync(TKey key, TValue value, double score, CancellationToken cancellationToken = default)
    {
        var redisKey = GetRedisKey(key);
        var redisValue = SerializeValue(value);

        await _db.SortedSetAddAsync(redisKey, redisValue, score);
        await _db.SortedSetRemoveRangeByRankAsync(redisKey, 0, -_maxEntries - 1);
        await _db.KeyExpireAsync(redisKey, _expiration);
    }

    public virtual async Task<IReadOnlyList<TValue>> GetRecentAsync(TKey key, int count, CancellationToken cancellationToken = default)
    {
        var redisKey = GetRedisKey(key);
        var entries = await _db.SortedSetRangeByRankAsync(redisKey, -count, -1, Order.Descending);

        return entries
            .Select(entry => DeserializeValue(entry))
            .ToList();
    }

    public virtual async Task RemoveAsync(TKey key, TValue value, CancellationToken cancellationToken = default)
    {
        var redisKey = GetRedisKey(key);
        var redisValue = SerializeValue(value);
        await _db.SortedSetRemoveAsync(redisKey, redisValue);
    }

    public virtual async Task ClearAsync(TKey key, CancellationToken cancellationToken = default)
    {
        var redisKey = GetRedisKey(key);
        await _db.KeyDeleteAsync(redisKey);
    }

    public virtual async Task<long> CountAsync(TKey key, CancellationToken cancellationToken = default)
    {
        var redisKey = GetRedisKey(key);
        return await _db.SortedSetLengthAsync(redisKey);
    }

    public virtual async Task<bool> ExistsAsync(TKey key, TValue value, CancellationToken cancellationToken = default)
    {
        var redisKey = GetRedisKey(key);
        var redisValue = SerializeValue(value);
        var score = await _db.SortedSetScoreAsync(redisKey, redisValue);
        return score.HasValue;
    }

    /// <summary>
    /// Constructs the full Redis key: {prefix}{key}
    /// </summary>
    protected virtual string GetRedisKey(TKey key) => $"{_keyPrefix}{key}";

    protected abstract string SerializeValue(TValue value);
    protected abstract TValue DeserializeValue(RedisValue redisValue);
}
