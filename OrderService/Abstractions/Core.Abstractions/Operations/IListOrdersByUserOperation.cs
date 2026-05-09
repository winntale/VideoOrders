using Core.Abstractions.OperationModels;

namespace Core.Abstractions.Operations;

public interface IListOrdersByUserOperation
{
    Task<Result<IReadOnlyList<OrderDetailsOperationModel>>> ExecuteAsync(
        ListOrdersByUserOperationModel operationModel,
        CancellationToken cancellationToken);
}
