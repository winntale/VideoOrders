using Core.Abstractions.OperationModels;

namespace Core.Abstractions.Operations;

public interface IStreamArchiveFileOperation
{
    Task<Result<FileDownloadOperationResult>> ExecuteAsync(
        StreamArchiveFileOperationModel model,
        CancellationToken cancellationToken);
}