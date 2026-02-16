namespace ToptalFinialSolution.Domain.Interfaces;

/// <summary>
/// Base interface for Redis repositories using Sorted Sets for time-based ordering.
/// </summary>
public interface IRedisRepository<TKey, TValue>
{
    Task AddAsync(TKey key, TValue value, CancellationToken cancellationToken = default);
    Task AddAsync(TKey key, TValue value, double score, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TValue>> GetRecentAsync(TKey key, int count, CancellationToken cancellationToken = default);
    Task RemoveAsync(TKey key, TValue value, CancellationToken cancellationToken = default);
    Task ClearAsync(TKey key, CancellationToken cancellationToken = default);
    Task<long> CountAsync(TKey key, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(TKey key, TValue value, CancellationToken cancellationToken = default);
}
