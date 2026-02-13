using Microsoft.EntityFrameworkCore;
using ToptalFinialSolution.Domain.Entities;
using ToptalFinialSolution.Domain.Interfaces;
using ToptalFinialSolution.Infrastructure.Data;

namespace ToptalFinialSolution.Infrastructure.Repositories;

public class ViewedRestaurantRepository : Repository<ViewedRestaurant>, IViewedRestaurantRepository
{
    public ViewedRestaurantRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Restaurant>> GetRecentlyViewedAsync(Guid userId, int count = 10)
    {
        return await _dbSet
            .Where(vr => vr.UserId == userId)
            .OrderByDescending(vr => vr.ViewedAt)
            .Select(vr => vr.Restaurant)
            .Include(r => r.Owner)
            .Distinct()
            .Take(count)
            .ToListAsync();
    }

    public async Task RecordViewAsync(Guid userId, Guid restaurantId)
    {
        var existingView = await _dbSet
            .FirstOrDefaultAsync(vr => vr.UserId == userId && vr.RestaurantId == restaurantId);

        if (existingView != null)
        {
            existingView.ViewedAt = DateTime.UtcNow;
            _dbSet.Update(existingView);
        }
        else
        {
            await _dbSet.AddAsync(new ViewedRestaurant
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RestaurantId = restaurantId,
                ViewedAt = DateTime.UtcNow
            });
        }
    }
}
