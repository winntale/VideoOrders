using Dal.Abstractions.Common;
using Dal.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dal.Common;

internal sealed class UnitOfWork(ResourceDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDisposableTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        return new DisposableTransaction(transaction);
    }

    private sealed class DisposableTransaction(IDbContextTransaction inner) : IDisposableTransaction
    {
        public Task CommitAsync(CancellationToken cancellationToken) => inner.CommitAsync(cancellationToken);

        public ValueTask DisposeAsync() => inner.DisposeAsync();
    }
}
