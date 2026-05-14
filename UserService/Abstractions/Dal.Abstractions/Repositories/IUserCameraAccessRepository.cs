namespace Dal.Abstractions.Repositories;

public interface IUserCameraAccessRepository
{
    Task<bool> HasAccessAsync(
        Guid userId,
        Guid cameraId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Guid>> GetAccessibleCameraIdsAsync(
        Guid userId,
        CancellationToken cancellationToken);
}