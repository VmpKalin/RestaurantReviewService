namespace ToptalFinialSolution.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IRestaurantRepository Restaurants { get; }
    IReviewRepository Reviews { get; }
    IViewedRestaurantRepository ViewedRestaurants { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
