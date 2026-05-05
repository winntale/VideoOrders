using Core.Abstractions.OperationModels;

namespace Core.Abstractions.Operations;

public interface IDownloadArchiveFileOperation
{
    Task<Result<FileDownloadOperationResult>> ExecuteAsync(
        DownloadArchiveFileOperationModel model,
        CancellationToken cancellationToken);
}