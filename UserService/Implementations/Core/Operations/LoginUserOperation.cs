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
            return Error.Validation("Invalid login or password.");
        }

        if (user.Status != UserStatus.Active)
        {
            return Error.Forbidden("User is not active.");
        }

        return new AuthenticatedUserOperationModel
        {
            UserId = user.Id,
            Login = user.Login
        };
    }
}
