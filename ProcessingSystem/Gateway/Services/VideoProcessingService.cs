using Events.Abstractions.Models;
using Gateway.Options;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Gateway.Services;

public sealed class VideoProcessingService(
    IOptions<ArchiveStorageOptions> options,
    IPublishEndpoint publishEndpoint)
{
    public async Task CompleteOrderAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var rootPath = options.Value.RootPath;
        var outputFileName = $"archive-{orderId:N}.mp4";
        var outputPath = Path.Combine(rootPath, outputFileName);

        Directory.CreateDirectory(rootPath);

        var inputPath = "/app/input/test-camera.mp4";
        File.Copy(inputPath, outputPath, overwrite: true);

        var fileInfo = new FileInfo(outputPath);

        await publishEndpoint.Publish(new OrderCompletedEvent
        {
            OrderId = orderId,
            OriginalFileName = "Архив камеры.mp4",
            StoredFileName = outputFileName,
            ContentType = "video/mp4",
            FileSize = fileInfo.Length,
            CompletedAtUtc = DateTimeOffset.UtcNow
        }, cancellationToken);
    }
}