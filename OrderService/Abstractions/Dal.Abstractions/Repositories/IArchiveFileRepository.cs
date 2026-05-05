using Dal.Abstractions.Entities;

namespace Dal.Abstractions.Repositories;

public interface IArchiveFileRepository
{
    Task AddAsync(ArchiveFile entity, CancellationToken cancellationToken);
    Task<ArchiveFile?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
}