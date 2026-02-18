using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using ToptalFinialSolution.Domain.Entities;
using ToptalFinialSolution.Domain.Interfaces;
using ToptalFinialSolution.Infrastructure.Data;
using ToptalFinialSolution.Infrastructure.Repositories.Base;
using ToptalFinialSolution.Infrastructure.Repositories.Base.EF;

namespace ToptalFinialSolution.Infrastructure.Repositories;

public class RestaurantRepository(ApplicationDbContext context) : Repository<Restaurant>(context), IRestaurantRepository
{
    public async Task<(IReadOnlyList<Restaurant> Restaurants, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? titleFilter = null,
        double? latitude = null,
        double? longitude = null,
        double? radiusKm = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.Include(r => r.Owner).AsQueryable();

        if (!string.IsNullOrWhiteSpace(titleFilter))
        {
            query = query.Where(r => r.Title.Contains(titleFilter));
        }

        Point? searchPoint = null;

        if (latitude.HasValue && longitude.HasValue && radiusKm.HasValue)
        {
            var radiusMeters = radiusKm.Value * 1000;
            searchPoint = new Point(longitude.Value, latitude.Value) { SRID = 4326 };

            query = query.Where(r =>
                r.Location != null &&
                r.Location.IsWithinDistance(searchPoint, radiusMeters));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var orderedQuery = searchPoint is not null
            ? query.OrderBy(r => r.Location!.Distance(searchPoint))
            : query.OrderByDescending(r => r.CreatedAt);

        var restaurants = await orderedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (restaurants, totalCount);
    }

    public async Task<Restaurant?> GetByIdWithReviewsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(r => r.Owner)
            .Include(r => r.Reviews)
            .ThenInclude(rev => rev.Reviewer)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }
}
