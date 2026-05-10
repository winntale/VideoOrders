using Core.Abstractions.OperationModels;

namespace Core.Abstractions.Operations;

public interface ILoginUserOperation
{
    Task<Result<AuthenticatedUserOperationModel>> ExecuteAsync(
        LoginUserOperationModel operationModel,
        CancellationToken cancellationToken);
}
