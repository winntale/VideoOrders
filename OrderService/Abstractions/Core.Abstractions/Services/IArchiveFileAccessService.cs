using Core.Abstractions.OperationModels;

namespace Core.Abstractions.Services;

public interface IArchiveFileAccessService
{
    Task<Result<FileDownloadOperationResult>> GetFileAsync(
        Guid orderId,
        CancellationToken cancellationToken);
}