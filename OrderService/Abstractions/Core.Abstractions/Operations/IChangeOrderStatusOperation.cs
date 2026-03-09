using Core.Abstractions.OperationModels;

namespace Core.Abstractions.Operations;

public interface IChangeOrderStatusOperation
{
    Task<Result<OrderDetailsOperationModel>> ExecuteAsync(
        ChangeOrderStatusOperationModel operationModel,
        CancellationToken cancellationToken);
}