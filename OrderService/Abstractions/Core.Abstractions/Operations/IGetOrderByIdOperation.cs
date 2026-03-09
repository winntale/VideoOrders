using Core.Abstractions.OperationModels;

namespace Core.Abstractions.Operations;

public interface IGetOrderByIdOperation
{
    Task<Result<OrderDetailsOperationModel>> GetOrderByIdAsync(
        GetOrderByIdOperationModel operationModel,
        CancellationToken cancellationToken);
}