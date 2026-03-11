namespace Dal.Abstractions.Repositories;

public interface IUserCameraAccessRepository
{
    Task<bool> HasAccessAsync(
        Guid userId,
        Guid cameraId,
        CancellationToken cancellationToken);
}