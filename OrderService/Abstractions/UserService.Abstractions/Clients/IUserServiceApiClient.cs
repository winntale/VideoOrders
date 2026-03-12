using UserService.Abstractions.Models;

namespace UserService.Abstractions.Clients;

public interface IUserServiceApiClient
{
    Task<ValidateAccessResultClientModel?> ValidateUserAccessAsync(
        ValidateAccessClientModel model,
        CancellationToken cancellationToken);
}