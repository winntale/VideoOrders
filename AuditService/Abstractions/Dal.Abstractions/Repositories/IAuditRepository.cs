using Dal.Abstractions.Entities;

namespace Dal.Abstractions.Repositories;

public interface IAuditRepository
{
    Task AddAsync(AuditEntry entry, CancellationToken cancellationToken);
    Task<IReadOnlyList<AuditEntry>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
}