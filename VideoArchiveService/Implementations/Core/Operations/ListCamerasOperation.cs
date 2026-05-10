using Core.Abstractions;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Dal.Abstractions.Repositories;

namespace Core.Operations;

internal sealed class ListCamerasOperation(
    ICameraRepository cameraRepository,
    IVideoSegmentRepository videoSegmentRepository) : IListCamerasOperation
{
    public async Task<Result<IReadOnlyList<CameraOperationModel>>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var cameras = await cameraRepository.ListAsync(cancellationToken);
        if (cameras.Count == 0)
        {
            return Result<IReadOnlyList<CameraOperationModel>>.Success(Array.Empty<CameraOperationModel>());
        }

        var cameraIds = cameras.Select(c => c.Id).ToArray();
        var segments = await videoSegmentRepository.ListByCameraIdsAsync(cameraIds, cancellationToken);

        var segmentsByCamera = segments
            .GroupBy(s => s.CameraId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<SegmentRangeOperationModel>)g
                .Select(s => new SegmentRangeOperationModel
                {
                    FromUtc = s.FromUtc,
                    ToUtc = s.ToUtc
                })
                .ToArray());

        var models = cameras
            .Select(c => new CameraOperationModel
            {
                Id = c.Id,
                Name = c.Name,
                IsActive = c.IsActive,
                Segments = segmentsByCamera.TryGetValue(c.Id, out var s)
                    ? s
                    : Array.Empty<SegmentRangeOperationModel>()
            })
            .ToArray();

        return Result<IReadOnlyList<CameraOperationModel>>.Success(models);
    }
}
