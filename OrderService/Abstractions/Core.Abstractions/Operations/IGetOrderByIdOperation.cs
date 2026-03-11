using Core.Abstractions.OperationModels;

namespace Core.Abstractions.Operations;

public interface IGetOrderByIdOperation
{
    Task<Result<OrderDetailsOperationModel>> ExecuteAsync(
        GetOrderByIdOperationModel operationModel,
        CancellationToken cancellationToken);
}