using Microsoft.EntityFrameworkCore;
using ToptalFinialSolution.Domain.Entities;
using ToptalFinialSolution.Domain.Interfaces;
using ToptalFinialSolution.Infrastructure.Data;
using ToptalFinialSolution.Infrastructure.Repositories.Base;
using ToptalFinialSolution.Infrastructure.Repositories.Base.EF;

namespace ToptalFinialSolution.Infrastructure.Repositories;

public class ReviewRepository(ApplicationDbContext context) : Repository<Review>(context), IReviewRepository
{
    public async Task<(IReadOnlyList<Review> Reviews, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        Guid? restaurantId = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(r => r.Restaurant)
            .Include(r => r.Reviewer)
            .AsQueryable();

        if (restaurantId.HasValue)
        {
            query = query.Where(r => r.RestaurantId == restaurantId.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (reviews, totalCount);
    }

    public async Task<double> GetAverageRatingByRestaurantAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        var query = DbSet.Where(r => r.RestaurantId == restaurantId);

        if (!await query.AnyAsync(cancellationToken))
            return 0;

        return await query.AverageAsync(r => r.Rating, cancellationToken);
    }

    public async Task<int> GetReviewCountByRestaurantAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        return await DbSet.CountAsync(r => r.RestaurantId == restaurantId, cancellationToken);
    }
}
