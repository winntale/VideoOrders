using Dal.Abstractions.Entities;

namespace Dal.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken);

    Task AddAsync(User user, CancellationToken cancellationToken);
}