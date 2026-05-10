using Core.Abstractions.OperationModels;

namespace Core.Abstractions.Operations;

public interface IRegisterUserOperation
{
    Task<Result<AuthenticatedUserOperationModel>> ExecuteAsync(
        RegisterUserOperationModel operationModel,
        CancellationToken cancellationToken);
}
