using ToptalFinialSolution.Domain.Entities;

namespace ToptalFinialSolution.Domain.Interfaces;

public interface IRestaurantRepository : IRepository<Restaurant>
{
    Task<(IReadOnlyList<Restaurant> Restaurants, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? titleFilter = null,
        double? latitude = null,
        double? longitude = null,
        double? radiusKm = null,
        CancellationToken cancellationToken = default);

    Task<Restaurant?> GetByIdWithReviewsAsync(Guid id, CancellationToken cancellationToken = default);
}
