namespace ToptalFinialSolution.Domain.Interfaces;

/// <summary>
/// Base interface for Redis repositories using Sorted Sets for time-based ordering
/// </summary>
/// <typeparam name="TKey">The type of the key (e.g., Guid for User ID)</typeparam>
/// <typeparam name="TValue">The type of the value (e.g., Guid for Restaurant ID)</typeparam>
public interface IRedisRepository<TKey, TValue>
{
    /// <summary>
    /// Adds or updates an entry with the current timestamp
    /// </summary>
    Task AddAsync(TKey key, TValue value);
    
    /// <summary>
    /// Adds or updates an entry with a specific score/timestamp
    /// </summary>
    Task AddAsync(TKey key, TValue value, double score);
    
    /// <summary>
    /// Gets the most recent N entries
    /// </summary>
    Task<List<TValue>> GetRecentAsync(TKey key, int count);
    
    /// <summary>
    /// Removes a specific entry
    /// </summary>
    Task RemoveAsync(TKey key, TValue value);
    
    /// <summary>
    /// Removes all entries for a key
    /// </summary>
    Task ClearAsync(TKey key);
    
    /// <summary>
    /// Gets the count of entries for a key
    /// </summary>
    Task<long> CountAsync(TKey key);
    
    /// <summary>
    /// Checks if a specific entry exists
    /// </summary>
    Task<bool> ExistsAsync(TKey key, TValue value);
}
