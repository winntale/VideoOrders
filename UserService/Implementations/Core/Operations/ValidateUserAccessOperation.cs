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
                $"Пользователь с идентификатором '{operationModel.UserId}' не найден.");
        }

        if (user.Status != UserStatus.Active)
        {
            return new UserAccessValidationResultOperationModel
            {
                IsAllowed = false,
                DenyReason = "Пользователь не активен."
            };
        }

        var hasAccess = await userCameraAccessRepository.HasAccessAsync(
            operationModel.UserId,
            operationModel.CameraId,
            cancellationToken);

        var resultModel = new UserAccessValidationResultOperationModel
        {
            IsAllowed = hasAccess,
            DenyReason = hasAccess ? null : "У пользователя нет доступа к этой камере."
        };

        return resultModel;
    }
}