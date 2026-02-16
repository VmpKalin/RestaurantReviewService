using StackExchange.Redis;
using ToptalFinialSolution.Domain.Interfaces;

namespace ToptalFinialSolution.Infrastructure.Repositories;

/// <summary>
/// Base Redis repository implementation using Sorted Sets for time-based ordering
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

    public virtual async Task AddAsync(TKey key, TValue value)
    {
        var score = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        await AddAsync(key, value, score);
    }

    public virtual async Task AddAsync(TKey key, TValue value, double score)
    {
        var redisKey = GetRedisKey(key);
        var redisValue = SerializeValue(value);
        
        // Add or update the value with the score
        await _db.SortedSetAddAsync(redisKey, redisValue, score);
        
        // Keep only the most recent maxEntries
        await _db.SortedSetRemoveRangeByRankAsync(redisKey, 0, -_maxEntries - 1);
        
        // Set expiration
        await _db.KeyExpireAsync(redisKey, _expiration);
    }

    public virtual async Task<List<TValue>> GetRecentAsync(TKey key, int count)
    {
        var redisKey = GetRedisKey(key);
        
        // Get the most recent entries (highest scores = most recent timestamps)
        var entries = await _db.SortedSetRangeByRankAsync(redisKey, -count, -1, Order.Descending);
        
        return entries
            .Select(entry => DeserializeValue(entry))
            .ToList();
    }

    public virtual async Task RemoveAsync(TKey key, TValue value)
    {
        var redisKey = GetRedisKey(key);
        var redisValue = SerializeValue(value);
        await _db.SortedSetRemoveAsync(redisKey, redisValue);
    }

    public virtual async Task ClearAsync(TKey key)
    {
        var redisKey = GetRedisKey(key);
        await _db.KeyDeleteAsync(redisKey);
    }

    public virtual async Task<long> CountAsync(TKey key)
    {
        var redisKey = GetRedisKey(key);
        return await _db.SortedSetLengthAsync(redisKey);
    }

    public virtual async Task<bool> ExistsAsync(TKey key, TValue value)
    {
        var redisKey = GetRedisKey(key);
        var redisValue = SerializeValue(value);
        var score = await _db.SortedSetScoreAsync(redisKey, redisValue);
        return score.HasValue;
    }

    /// <summary>
    /// Constructs the full Redis key from the prefix and the provided key
    /// </summary>
    protected virtual string GetRedisKey(TKey key)
    {
        return $"{_keyPrefix}{key}";
    }

    /// <summary>
    /// Serializes the value to a Redis-compatible string
    /// Override for custom serialization logic
    /// </summary>
    protected abstract string SerializeValue(TValue value);

    /// <summary>
    /// Deserializes the Redis string back to the value type
    /// Override for custom deserialization logic
    /// </summary>
    protected abstract TValue DeserializeValue(RedisValue redisValue);
}
