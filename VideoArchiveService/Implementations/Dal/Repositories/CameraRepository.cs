using AutoMapper;
using Dal.Abstractions.Entities;
using Dal.Abstractions.Models;
using Dal.Abstractions.Repositories;
using Dal.Context;
using Microsoft.EntityFrameworkCore;

namespace Dal.Repositories;

internal sealed class CameraRepository(VideoArchiveDbContext dbContext, IMapper mapper) : ICameraRepository
{
    public Task<Camera?> GetByIdAsync(CameraRepositoryModel cameraRepositoryModel, CancellationToken cancellationToken)
    {
        return dbContext.Cameras
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == cameraRepositoryModel.CameraId, cancellationToken);
    }
}