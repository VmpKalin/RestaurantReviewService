using ToptalFinialSolution.Domain.Entities;

namespace ToptalFinialSolution.Domain.Interfaces;

public interface IRestaurantRepository : IRepository<Restaurant>
{
    Task<(IEnumerable<Restaurant> Restaurants, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? titleFilter = null, 
        double? latitude = null, 
        double? longitude = null, 
        double? radiusKm = null);
    
    Task<Restaurant?> GetByIdWithReviewsAsync(Guid id);
}
