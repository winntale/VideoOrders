using AutoMapper;
using Core.Abstractions;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Dal.Abstractions.Enums;
using Dal.Abstractions.Repositories;

namespace Core.Operations;

internal sealed class ValidateUserAccessOperation(
    IUserRepository userRepository,
    IUserCameraAccessRepository userCameraAccessRepository,
    IMapper mapper)
    : IValidateUserAccessOperation
{
    public async Task<Result<UserAccessValidationResultOperationModel>> ExecuteAsync(
        ValidateUserAccessOperationModel operationModel,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(operationModel.UserId, cancellationToken);

        if (user is null)
        {
            return Error.NotFound(
                $"User with id '{operationModel.UserId}' was not found.");
        }

        if (user.Status != UserStatus.Active)
        {
            return new UserAccessValidationResultOperationModel
            {
                UserId = operationModel.UserId,
                CameraId = operationModel.CameraId,
                IsAllowed = false,
                DenyReason = "User is not active."
            };
        }

        var hasAccess = await userCameraAccessRepository.HasAccessAsync(
            operationModel.UserId,
            operationModel.CameraId,
            cancellationToken);

        var resultModel = new UserAccessValidationResultOperationModel
        {
            UserId = operationModel.UserId,
            CameraId = operationModel.CameraId,
            IsAllowed = hasAccess,
            DenyReason = hasAccess ? null : "User has no access to this camera."
        };

        return resultModel;
    }
}