using Dal.Abstractions.Entities;
using Dal.Abstractions.Enums;
using Dal.Abstractions.Repositories;
using Dal.Context;
using Microsoft.EntityFrameworkCore;

namespace Dal.Repositories;

internal sealed class ResourceRepository(ResourceDbContext dbContext) : IResourceRepository
{
    public async Task<IReadOnlyList<Resource>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Resources.ToListAsync(cancellationToken);
    }

    public Task<Resource?> GetByTypeAsync(ResourceType type, CancellationToken cancellationToken)
    {
        return dbContext.Resources.FirstOrDefaultAsync(x => x.Type == type, cancellationToken);
    }
}
