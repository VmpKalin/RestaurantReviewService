using Microsoft.EntityFrameworkCore;
using ToptalFinialSolution.Domain.Entities;
using ToptalFinialSolution.Domain.Interfaces;
using ToptalFinialSolution.Infrastructure.Data;

namespace ToptalFinialSolution.Infrastructure.Repositories;

public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Review> Reviews, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        Guid? restaurantId = null)
    {
        var query = _dbSet
            .Include(r => r.Restaurant)
            .Include(r => r.Reviewer)
            .AsQueryable();

        if (restaurantId.HasValue)
        {
            query = query.Where(r => r.RestaurantId == restaurantId.Value);
        }

        var totalCount = await query.CountAsync();
        
        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (reviews, totalCount);
    }

    public async Task<double> GetAverageRatingByRestaurantAsync(Guid restaurantId)
    {
        var reviews = await _dbSet.Where(r => r.RestaurantId == restaurantId).ToListAsync();
        return reviews.Any() ? reviews.Average(r => r.Rating) : 0;
    }

    public async Task<int> GetReviewCountByRestaurantAsync(Guid restaurantId)
    {
        return await _dbSet.CountAsync(r => r.RestaurantId == restaurantId);
    }
}
