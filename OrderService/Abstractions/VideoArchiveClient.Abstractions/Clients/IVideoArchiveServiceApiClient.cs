using VideoArchiveClient.Abstractions.Models;

namespace VideoArchiveClient.Abstractions.Clients;

public interface IVideoArchiveServiceApiClient
{
    Task<Result<ValidateArchiveAvailabilityResultClientModel?>> ValidateArchiveAvailabilityAsync(
        ValidateArchiveAvailabilityClientModel clientModel,
        CancellationToken cancellationToken);
}