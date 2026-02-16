using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using ToptalFinialSolution.Domain.Entities;
using ToptalFinialSolution.Domain.Interfaces;
using ToptalFinialSolution.Infrastructure.Data;

namespace ToptalFinialSolution.Infrastructure.Repositories;

public class RestaurantRepository(ApplicationDbContext context) : Repository<Restaurant>(context), IRestaurantRepository
{
    public async Task<(IEnumerable<Restaurant> Restaurants, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        string? titleFilter = null, 
        double? latitude = null, 
        double? longitude = null, 
        double? radiusKm = null)
    {
        var query = _dbSet.Include(r => r.Owner).AsQueryable();

        // Apply title filter
        if (!string.IsNullOrWhiteSpace(titleFilter))
        {
            query = query.Where(r => r.Title.Contains(titleFilter));
        }

        Point? searchPoint = null;

        // Apply location filter using PostGIS ST_DWithin (uses GIST index)
        if (latitude.HasValue && longitude.HasValue && radiusKm.HasValue)
        {
            var radiusMeters = radiusKm.Value * 1000;
            searchPoint = new Point(longitude.Value, latitude.Value) { SRID = 4326 };

            query = query.Where(r =>
                r.Location != null &&
                r.Location.IsWithinDistance(searchPoint, radiusMeters));
        }

        var totalCount = await query.CountAsync();

        // Order by distance when searching by location, otherwise by creation date
        var orderedQuery = searchPoint != null
            ? query.OrderBy(r => r.Location!.Distance(searchPoint))
            : query.OrderByDescending(r => r.CreatedAt);

        var restaurants = await orderedQuery
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
