using UserServiceClient.Abstractions.Models;

namespace UserServiceClient.Abstractions.Clients;

public interface IUserServiceApiClient
{
    Task<Result<ValidateAccessResultClientModel?>> ValidateUserAccessAsync(
        ValidateAccessClientModel model,
        CancellationToken cancellationToken);
}