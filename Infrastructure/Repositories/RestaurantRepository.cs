using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
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

        // Apply location filter using PostGIS ST_DWithin (uses GIST index)
        if (latitude.HasValue && longitude.HasValue && radiusInMiles.HasValue)
        {
            var radiusInMeters = radiusInMiles.Value * 1609.34;
            var searchPoint = new Point(longitude.Value, latitude.Value) { SRID = 4326 };

            query = query.Where(r =>
                r.Location != null &&
                r.Location.IsWithinDistance(searchPoint, radiusInMeters));
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
