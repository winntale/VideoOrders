using AutoMapper;
using Core.Abstractions;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Dal.Abstractions.Repositories;

namespace Core.Operations;

internal sealed class ListCamerasOperation(
    ICameraRepository cameraRepository,
    IMapper mapper) : IListCamerasOperation
{
    public async Task<Result<IReadOnlyList<CameraOperationModel>>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var cameras = await cameraRepository.ListAsync(cancellationToken);
        var models = mapper.Map<IReadOnlyList<CameraOperationModel>>(cameras);
        return Result<IReadOnlyList<CameraOperationModel>>.Success(models);
    }
}
