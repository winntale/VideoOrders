using AutoMapper;
using Dal.Abstractions.Models;
using Dal.Abstractions.Repositories;
using Dal.Context;
using Microsoft.EntityFrameworkCore;

namespace Dal.Repositories;

internal sealed class VideoSegmentRepository(VideoArchiveDbContext dbContext, IMapper mapper) : IVideoSegmentRepository
{
    public Task<bool> ExistsCoveringSegmentAsync(VideoSegmentRepositoryModel repositoryModel, CancellationToken cancellationToken)
    {
        return dbContext.VideoSegments
            .AsNoTracking()
            .AnyAsync(
                x => x.CameraId == repositoryModel.CameraId
                     && x.FromUtc <= repositoryModel.FromUtc
                     && x.ToUtc >= repositoryModel.ToUtc,
                cancellationToken);
    }
}