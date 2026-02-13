using ToptalFinialSolution.Domain.Entities;

namespace ToptalFinialSolution.Domain.Interfaces;

public interface IReviewRepository : IRepository<Review>
{
    Task<(IEnumerable<Review> Reviews, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        Guid? restaurantId = null);
    
    Task<double> GetAverageRatingByRestaurantAsync(Guid restaurantId);
    Task<int> GetReviewCountByRestaurantAsync(Guid restaurantId);
}
