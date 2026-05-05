using Core.Abstractions;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Core.Abstractions.Services;

namespace Core.Operations;

internal sealed class StreamArchiveFileOperation(
    IArchiveFileAccessService accessService)
    : IStreamArchiveFileOperation
{
    public async Task<Result<FileDownloadOperationResult>> ExecuteAsync(
        StreamArchiveFileOperationModel model,
        CancellationToken cancellationToken)
    {
        return await accessService.GetFileAsync(model.OrderId, cancellationToken);
    }
}