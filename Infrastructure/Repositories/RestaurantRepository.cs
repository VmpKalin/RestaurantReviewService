using Microsoft.EntityFrameworkCore;
using ToptalFinialSolution.Domain.Entities;
using ToptalFinialSolution.Domain.Interfaces;
using ToptalFinialSolution.Infrastructure.Data;

namespace ToptalFinialSolution.Infrastructure.Repositories;

public class RestaurantRepository : Repository<Restaurant>, IRestaurantRepository
{
    public RestaurantRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IEnumerable<Restaurant> Restaurants, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? titleFilter = null, 
        double? latitude = null, 
        double? longitude = null, 
        double? radiusInMiles = null)
    {
        var query = _dbSet.Include(r => r.Owner).AsQueryable();

        // Apply title filter
        if (!string.IsNullOrWhiteSpace(titleFilter))
        {
            query = query.Where(r => r.Title.Contains(titleFilter));
        }

        // Apply location filter (Haversine formula)
        if (latitude.HasValue && longitude.HasValue && radiusInMiles.HasValue)
        {
            var radiusInKm = radiusInMiles.Value * 1.60934;
            query = query.Where(r =>
                (6371 * Math.Acos(
                    Math.Cos(latitude.Value * Math.PI / 180) *
                    Math.Cos(r.Latitude * Math.PI / 180) *
                    Math.Cos((r.Longitude - longitude.Value) * Math.PI / 180) +
                    Math.Sin(latitude.Value * Math.PI / 180) *
                    Math.Sin(r.Latitude * Math.PI / 180)
                )) <= radiusInKm
            );
        }

        var totalCount = await query.CountAsync();
        
        var restaurants = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (restaurants, totalCount);
    }

    public async Task<Restaurant?> GetByIdWithReviewsAsync(Guid id)
    {
        return await _dbSet
            .Include(r => r.Owner)
            .Include(r => r.Reviews)
            .ThenInclude(rev => rev.Reviewer)
            .FirstOrDefaultAsync(r => r.Id == id);
    }
}
