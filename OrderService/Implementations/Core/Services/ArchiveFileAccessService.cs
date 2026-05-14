using Core.Abstractions;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Services;
using Dal.Abstractions.Repositories;

namespace Core.Services;

internal sealed class ArchiveFileAccessService(
    IArchiveFileRepository archiveFileRepository,
    IArchiveFileStorage archiveFileStorage)
    : IArchiveFileAccessService
{
    public async Task<Result<FileDownloadOperationResult>> GetFileAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var archiveFile = await archiveFileRepository.GetByOrderIdAsync(orderId, cancellationToken);

        if (archiveFile is null)
        {
            return Error.NotFound($"Файл архива для заказа '{orderId}' не найден.");
        }

        if (!archiveFileStorage.Exists(archiveFile.StoragePath))
        {
            return Error.NotFound($"Физический файл архива для заказа '{orderId}' отсутствует.");
        }

        var stream = archiveFileStorage.OpenRead(archiveFile.StoragePath);
        
        return new FileDownloadOperationResult
        {
            Stream = stream,
            ContentType = archiveFile.ContentType,
            FileName = archiveFile.OriginalFileName,
            FileSize = archiveFile.FileSize
        };
    }
}