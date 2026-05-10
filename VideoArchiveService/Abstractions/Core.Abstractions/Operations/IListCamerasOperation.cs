using Core.Abstractions.OperationModels;

namespace Core.Abstractions.Operations;

public interface IListCamerasOperation
{
    Task<Result<IReadOnlyList<CameraOperationModel>>> ExecuteAsync(CancellationToken cancellationToken);
}
