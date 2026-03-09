using Dal.Abstractions.Common;
using Dal.Context;

namespace Dal.Common;

internal sealed class UnitOfWork(OrderDbContext dbContext)
    : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}