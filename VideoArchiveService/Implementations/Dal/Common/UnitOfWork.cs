using Dal.Abstractions.Common;
using Dal.Context;

namespace Dal.Common;

internal sealed class UnitOfWork(VideoArchiveDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}