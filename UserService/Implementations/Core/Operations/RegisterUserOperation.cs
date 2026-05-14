using Core.Abstractions;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Dal.Abstractions.Common;
using Dal.Abstractions.Entities;
using Dal.Abstractions.Enums;
using Dal.Abstractions.Repositories;

namespace Core.Operations;

internal sealed class RegisterUserOperation(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRegisterUserOperation
{
    public async Task<Result<AuthenticatedUserOperationModel>> ExecuteAsync(
        RegisterUserOperationModel operationModel,
        CancellationToken cancellationToken)
    {
        var login = operationModel.Login.Trim();

        if (login.Length < 3)
        {
            return Error.Validation("Логин должен содержать минимум 3 символа.");
        }

        if (operationModel.Password.Length < 6)
        {
            return Error.Validation("Пароль должен содержать минимум 6 символов.");
        }

        var existing = await userRepository.GetByLoginAsync(login, cancellationToken);
        if (existing is not null)
        {
            return Error.Conflict($"Пользователь с логином '{login}' уже существует.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(operationModel.Password),
            Status = UserStatus.Active
        };

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthenticatedUserOperationModel
        {
            UserId = user.Id,
            Login = user.Login
        };
    }
}
