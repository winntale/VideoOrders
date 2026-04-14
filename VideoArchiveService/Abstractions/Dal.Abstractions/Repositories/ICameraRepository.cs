using Dal.Abstractions.Entities;
using Dal.Abstractions.Models;

namespace Dal.Abstractions.Repositories;

public interface ICameraRepository
{
    Task<Camera?> GetByIdAsync(CameraRepositoryModel cameraRepositoryModel, CancellationToken cancellationToken);
}