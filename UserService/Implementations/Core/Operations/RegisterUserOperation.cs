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
            return Error.Validation("Login must be at least 3 characters.");
        }

        if (operationModel.Password.Length < 6)
        {
            return Error.Validation("Password must be at least 6 characters.");
        }

        var existing = await userRepository.GetByLoginAsync(login, cancellationToken);
        if (existing is not null)
        {
            return Error.Conflict($"User with login '{login}' already exists.");
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
