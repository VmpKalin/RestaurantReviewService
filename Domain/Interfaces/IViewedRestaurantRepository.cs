using ToptalFinialSolution.Domain.Entities;

namespace ToptalFinialSolution.Domain.Interfaces;

public interface IViewedRestaurantRepository : IRepository<ViewedRestaurant>
{
    Task<IEnumerable<Restaurant>> GetRecentlyViewedAsync(Guid userId, int count = 10);
    Task RecordViewAsync(Guid userId, Guid restaurantId);
}
