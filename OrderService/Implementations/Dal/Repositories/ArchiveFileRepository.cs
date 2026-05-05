using Dal.Abstractions.Entities;
using Dal.Abstractions.Repositories;
using Dal.Context;
using Microsoft.EntityFrameworkCore;

namespace Dal.Repositories;

internal sealed class ArchiveFileRepository(OrderDbContext context) : IArchiveFileRepository
{
    public async Task AddAsync(ArchiveFile entity, CancellationToken cancellationToken)
    {
        await context.ArchiveFiles.AddAsync(entity, cancellationToken);
    }

    public Task<ArchiveFile?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return context.ArchiveFiles.FirstOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);
    }
}