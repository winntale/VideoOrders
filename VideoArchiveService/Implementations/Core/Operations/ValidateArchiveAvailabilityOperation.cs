using AutoMapper;
using Core.Abstractions;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Dal.Abstractions.Models;
using Dal.Abstractions.Repositories;

namespace Core.Operations;

internal sealed class ValidateArchiveAvailabilityOperation(
    ICameraRepository cameraRepository,
    IVideoSegmentRepository videoSegmentRepository,
    IMapper mapper)
    : IValidateArchiveAvailabilityOperation
{
    public async Task<Result<ArchiveAvailabilityResultOperationModel>> ExecuteAsync(
        ValidateArchiveAvailabilityOperationModel operationModel,
        CancellationToken cancellationToken)
    {
        var cameraRepositoryModel = mapper.Map<CameraRepositoryModel>(operationModel);
        
        var camera = await cameraRepository.GetByIdAsync(
            cameraRepositoryModel,
            cancellationToken);

        if (camera is null)
        {
            return Error.NotFound(
                $"Camera with id '{operationModel.CameraId}' was not found.");
        }

        if (!camera.IsActive)
        {
            return new ArchiveAvailabilityResultOperationModel
            {
                IsAvailable = false,
                DenyReason = "Camera is inactive."
            };
        }

        var videoSegmentRepositoryModel = mapper.Map<VideoSegmentRepositoryModel>(operationModel);
        
        var isAvailable = await videoSegmentRepository.ExistsCoveringSegmentAsync(
            videoSegmentRepositoryModel,
            cancellationToken);

        var resultModel = new ArchiveAvailabilityResultOperationModel
        {
            IsAvailable = isAvailable,
            DenyReason = isAvailable
                ? null
                : "Archive for the specified time interval is unavailable."
        };

        return resultModel;
    }
}