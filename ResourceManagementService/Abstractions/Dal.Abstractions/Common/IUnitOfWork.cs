namespace Dal.Abstractions.Common;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    Task<IDisposableTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}

public interface IDisposableTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken);
}
