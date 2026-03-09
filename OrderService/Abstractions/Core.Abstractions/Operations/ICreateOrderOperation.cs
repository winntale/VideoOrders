using Core.Abstractions.OperationModels;

namespace Core.Abstractions.Operations;

public interface ICreateOrderOperation
{
    Task<Result<OrderDetailsOperationModel>> ExecuteAsync(
        CreateOrderOperationModel operationModel,
        CancellationToken cancellationToken);
}