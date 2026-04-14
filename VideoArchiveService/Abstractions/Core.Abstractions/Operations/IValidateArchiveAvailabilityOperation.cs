using Core.Abstractions.OperationModels;

namespace Core.Abstractions.Operations;

public interface IValidateArchiveAvailabilityOperation
{
    Task<Result<ArchiveAvailabilityResultOperationModel>> ExecuteAsync(
        ValidateArchiveAvailabilityOperationModel operationModel,
        CancellationToken cancellationToken);
}