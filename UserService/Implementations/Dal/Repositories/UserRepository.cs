using Dal.Abstractions.Entities;
using Dal.Abstractions.Repositories;
using Dal.Context;
using Microsoft.EntityFrameworkCore;

namespace Dal.Repositories;

internal sealed class UserRepository(UserDbContext dbContext)
    : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken)
    {
        return dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Login == login, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
    }
}