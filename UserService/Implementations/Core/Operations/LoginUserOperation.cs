using Core.Abstractions;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Dal.Abstractions.Enums;
using Dal.Abstractions.Repositories;

namespace Core.Operations;

internal sealed class LoginUserOperation(IUserRepository userRepository) : ILoginUserOperation
{
    public async Task<Result<AuthenticatedUserOperationModel>> ExecuteAsync(
        LoginUserOperationModel operationModel,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByLoginAsync(operationModel.Login, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(operationModel.Password, user.PasswordHash))
        {
            return Error.Validation("Неверный логин или пароль.");
        }

        if (user.Status != UserStatus.Active)
        {
            return Error.Forbidden("Пользователь не активен.");
        }

        return new AuthenticatedUserOperationModel
        {
            UserId = user.Id,
            Login = user.Login
        };
    }
}
