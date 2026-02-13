using ToptalFinialSolution.Domain.Entities;

namespace ToptalFinialSolution.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
