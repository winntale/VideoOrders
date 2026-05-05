using Core.Abstractions;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Core.Abstractions.Services;

namespace Core.Operations;

internal sealed class DownloadArchiveFileOperation(
    IArchiveFileAccessService accessService)
    : IDownloadArchiveFileOperation
{
    public async Task<Result<FileDownloadOperationResult>> ExecuteAsync(
        DownloadArchiveFileOperationModel model,
        CancellationToken cancellationToken)
    {
        return await accessService.GetFileAsync(model.OrderId, cancellationToken);
    }
}