using ToptalFinialSolution.Domain.Entities;

namespace ToptalFinialSolution.Domain.Interfaces;

public interface IReviewRepository : IRepository<Review>
{
    Task<(IReadOnlyList<Review> Reviews, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        Guid? restaurantId = null,
        CancellationToken cancellationToken = default);

    Task<double> GetAverageRatingByRestaurantAsync(Guid restaurantId, CancellationToken cancellationToken = default);
    Task<int> GetReviewCountByRestaurantAsync(Guid restaurantId, CancellationToken cancellationToken = default);
}
