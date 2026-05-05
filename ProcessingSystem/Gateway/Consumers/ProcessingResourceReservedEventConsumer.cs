
using Events.Abstractions;
using Events.Abstractions.Models;
using Gateway.Options;
using Microsoft.Extensions.Options;
using MassTransit;

namespace Gateway.Consumers;

public sealed class ProcessingResourceReservedEventConsumer(
    IOptions<ArchiveStorageOptions> storageOptions,
    IPublishEndpoint publishEndpoint)
    : IConsumer<ResourceReservedEvent>
{
    public async Task Consume(ConsumeContext<ResourceReservedEvent> context)
    {
        var message = context.Message;
        
        Console.WriteLine($"[ProcessingSystem] ResourceReserved received. OrderId: {message.OrderId}");

        await context.Publish(
            new ProcessingStartedEvent
            {
                OrderId = message.OrderId,
                StartedAtUtc = DateTimeOffset.UtcNow
            });

        await Task.Delay(TimeSpan.FromSeconds(2), context.CancellationToken);

        var isSuccess = message.OrderId.GetHashCode() % 5 != 0;

        if (isSuccess)
        {
            await CreateArchiveFileAndPublish(message.OrderId, context.CancellationToken);
        }
        else
        {
            await context.Publish(
                new OrderFailedEvent
                {
                    OrderId = message.OrderId,
                    Reason = "Processing failed due to internal pipeline error.",
                    FailedAtUtc = DateTimeOffset.UtcNow
                });
        }
    }

    private async Task CreateArchiveFileAndPublish(Guid orderId, CancellationToken cancellationToken)
    {
        var rootPath = storageOptions.Value.RootPath;
        var outputFileName = $"archive-{orderId:N}.mp4";
        var outputPath = Path.Combine(rootPath, outputFileName);

        Directory.CreateDirectory(rootPath);

        var inputPath = "/app/input/test-camera.mp4";
        
        if (!File.Exists(inputPath))
        {
            throw new FileNotFoundException($"Test video file not found: {inputPath}");
        }

        File.Copy(inputPath, outputPath, overwrite: true);

        var fileInfo = new FileInfo(outputPath);

        Console.WriteLine($"[ProcessingSystem] Archive file created: {outputPath}, size: {fileInfo.Length}");

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