using Dal.Abstractions.Models;

namespace Dal.Abstractions.Repositories;

public interface IVideoSegmentRepository
{
    Task<bool> ExistsCoveringSegmentAsync(
        VideoSegmentRepositoryModel repositoryModel,
        CancellationToken cancellationToken);
}