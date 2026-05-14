using Dal.Abstractions.Entities;
using Dal.Abstractions.Enums;

namespace Dal.Abstractions.Repositories;

public interface IResourceRepository
{
    Task<IReadOnlyList<Resource>> GetAllAsync(CancellationToken cancellationToken);

    Task<Resource?> GetByTypeAsync(ResourceType type, CancellationToken cancellationToken);
}
