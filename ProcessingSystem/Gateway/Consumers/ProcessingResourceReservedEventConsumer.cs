
using Events.Abstractions;
using Events.Abstractions.Models;
using Gateway.Options;
using MassTransit;
using Microsoft.Extensions.Options;
using System.Diagnostics;

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
        
        try
        {
            await CreateArchiveFileAndPublish(message, context.CancellationToken);
        }
        catch (Exception ex)
        {
            await context.Publish(
                new OrderFailedEvent
                {
                    OrderId = message.OrderId,
                    Reason = ex.Message,
                    FailedAtUtc = DateTimeOffset.UtcNow
                });
        }
    }

    private async Task CreateArchiveFileAndPublish(ResourceReservedEvent message, CancellationToken cancellationToken)
    {
        var rootPath = Environment.GetEnvironmentVariable("ArchiveStorage__RootPath") ?? "/app/storage/archive-results";
        var outputFileName = $"archive-{message.OrderId:N}.mp4";
        var outputPath = Path.Combine(rootPath, outputFileName);

        Directory.CreateDirectory(rootPath);

        var inputPath = Path.Combine("/app/input", $"{message.CameraId:D}.mp4");
        Console.WriteLine(inputPath);
        
        
        if (!File.Exists(inputPath))
        {
            throw new FileNotFoundException($"Source camera video file not found: {inputPath}");
        }

        var requestedDuration = message.ToUtc - message.FromUtc;
        if (requestedDuration <= TimeSpan.Zero)
        {
            throw new InvalidOperationException(
                $"Invalid processing interval. OrderId: {message.OrderId}, FromUtc: {message.FromUtc:O}, ToUtc: {message.ToUtc:O}");
        }

        await TrimVideoAsync(inputPath, outputPath, requestedDuration, cancellationToken);

        var fileInfo = new FileInfo(outputPath);

        Console.WriteLine($"[ProcessingSystem] Archive file created: {outputPath}, size: {fileInfo.Length}");

        await publishEndpoint.Publish(new OrderCompletedEvent
        {
            OrderId = message.OrderId,
            OriginalFileName = "Архив камеры.mp4",
            StoredFileName = outputFileName,
            ContentType = "video/mp4",
            FileSize = fileInfo.Length,
            CompletedAtUtc = DateTimeOffset.UtcNow
        }, cancellationToken);
    }

    private static async Task TrimVideoAsync(
        string inputPath,
        string outputPath,
        TimeSpan requestedDuration,
        CancellationToken cancellationToken)
    {
        var durationSeconds = requestedDuration.TotalSeconds.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);
        var arguments = $"-y -i \"{inputPath}\" -t {durationSeconds} -c:v libx264 -c:a aac \"{outputPath}\"";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var stdOutTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var stdErrTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        var stdOut = await stdOutTask;
        var stdErr = await stdErrTask;

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"ffmpeg failed with exit code {process.ExitCode}. StdOut: {stdOut}. StdErr: {stdErr}");
        }
    }
}