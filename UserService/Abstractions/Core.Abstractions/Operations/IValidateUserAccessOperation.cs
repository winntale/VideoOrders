using Core.Abstractions.OperationModels;

namespace Core.Abstractions.Operations;

public interface IValidateUserAccessOperation
{
    Task<Result<UserAccessValidationResultOperationModel>> ExecuteAsync(
        ValidateUserAccessOperationModel operationModel,
        CancellationToken cancellationToken);
}