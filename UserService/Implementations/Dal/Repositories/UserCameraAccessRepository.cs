using Dal.Abstractions.Repositories;
using Dal.Context;
using Microsoft.EntityFrameworkCore;

namespace Dal.Repositories;

internal sealed class UserCameraAccessRepository(UserDbContext dbContext)
    : IUserCameraAccessRepository
{
    public Task<bool> HasAccessAsync(
        Guid userId,
        Guid cameraId,
        CancellationToken cancellationToken)
    {
        return dbContext.UserCameraAccesses
            .AsNoTracking()
            .AnyAsync(
                x => x.UserId == userId && x.CameraId == cameraId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> GetAccessibleCameraIdsAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.UserCameraAccesses
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.CameraId)
            .ToListAsync(cancellationToken);
    }
}