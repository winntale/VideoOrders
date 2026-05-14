using System.Diagnostics;
using System.Globalization;
using Events.Abstractions.Models;
using Gateway.Options;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gateway.Consumers;

public sealed class ProcessingResourceReservedEventConsumer(
    IOptions<ArchiveStorageOptions> storageOptions,
    IPublishEndpoint publishEndpoint,
    ILogger<ProcessingResourceReservedEventConsumer> logger)
    : IConsumer<ResourceReservedEvent>
{
    private const int MaxReasonLength = 1000;

    public async Task Consume(ConsumeContext<ResourceReservedEvent> context)
    {
        var message = context.Message;
        logger.LogInformation(
            "ResourceReserved received. OrderId={OrderId} Duration={Duration}",
            message.OrderId, message.ToUtc - message.FromUtc);

        try
        {
            await PublishSafelyAsync(
                new ProcessingStartedEvent
                {
                    OrderId = message.OrderId,
                    StartedAtUtc = DateTimeOffset.UtcNow,
                },
                context.CancellationToken);

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(2), context.CancellationToken);
            }
            catch (OperationCanceledException)
            {
                // штатное завершение работы — не считаем за ошибку
                return;
            }

            await CreateArchiveFileAndPublish(message, context.CancellationToken);
        }
        catch (OperationCanceledException) when (context.CancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("Processing cancelled for OrderId={OrderId}", message.OrderId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Processing failed for OrderId={OrderId}", message.OrderId);

            // Публикуем уведомление об ошибке отдельным fire-and-forget вызовом
            // без cancellation token, чтобы сам Consume гарантированно не упал
            // и сообщение не ушло в _error при отмене.
            await PublishSafelyAsync(
                new OrderFailedEvent
                {
                    OrderId = message.OrderId,
                    Reason = TrimReason(ex.Message),
                    FailedAtUtc = DateTimeOffset.UtcNow,
                },
                CancellationToken.None);
        }
    }

    private async Task PublishSafelyAsync<TEvent>(TEvent evt, CancellationToken ct) where TEvent : class
    {
        try
        {
            await publishEndpoint.Publish(evt, ct);
        }
        catch (Exception ex)
        {
            // Если публикация не удалась — лучше залогировать и продолжить, чем
            // вернуть сообщение в _error: повторная обработка ничего не починит.
            logger.LogError(ex, "Failed to publish {EventType}.", typeof(TEvent).Name);
        }
    }

    private static string TrimReason(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return "Unknown error.";
        return raw.Length <= MaxReasonLength ? raw : raw[..MaxReasonLength] + "...";
    }

    private async Task CreateArchiveFileAndPublish(ResourceReservedEvent message, CancellationToken cancellationToken)
    {
        var rootPath = storageOptions.Value.RootPath
                       ?? Environment.GetEnvironmentVariable("ArchiveStorage__RootPath")
                       ?? "/app/storage/archive-results";
        var outputFileName = $"archive-{message.OrderId:N}.mp4";
        var outputPath = Path.Combine(rootPath, outputFileName);

        Directory.CreateDirectory(rootPath);

        var inputPath = Path.Combine("/app/input", $"{message.CameraId:D}.mp4");
        logger.LogInformation("Source camera file: {InputPath}", inputPath);

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

        var startOffset = message.SegmentStartUtc is { } segmentStart
            ? message.FromUtc - segmentStart
            : TimeSpan.Zero;

        if (startOffset < TimeSpan.Zero)
        {
            startOffset = TimeSpan.Zero;
        }

        await TrimVideoAsync(inputPath, outputPath, startOffset, requestedDuration, cancellationToken);

        var fileInfo = new FileInfo(outputPath);
        logger.LogInformation("Archive file created: {OutputPath} ({Size} bytes)", outputPath, fileInfo.Length);

        await publishEndpoint.Publish(new OrderCompletedEvent
        {
            OrderId = message.OrderId,
            OriginalFileName = "Архив камеры.mp4",
            StoredFileName = outputFileName,
            ContentType = "video/mp4",
            FileSize = fileInfo.Length,
            CompletedAtUtc = DateTimeOffset.UtcNow,
        }, cancellationToken);
    }

    private async Task TrimVideoAsync(
        string inputPath,
        string outputPath,
        TimeSpan startOffset,
        TimeSpan requestedDuration,
        CancellationToken cancellationToken)
    {
        var startSeconds = startOffset.TotalSeconds.ToString("0.###", CultureInfo.InvariantCulture);
        var durationSeconds = requestedDuration.TotalSeconds.ToString("0.###", CultureInfo.InvariantCulture);

        // Используем ArgumentList — это исключает проблемы экранирования
        // на путях с пробелами/спецсимволами и на длинных аргументах.
        var psi = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        psi.ArgumentList.Add("-y");
        if (startOffset > TimeSpan.Zero)
        {
            psi.ArgumentList.Add("-ss");
            psi.ArgumentList.Add(startSeconds);
        }
        psi.ArgumentList.Add("-i");
        psi.ArgumentList.Add(inputPath);
        psi.ArgumentList.Add("-t");
        psi.ArgumentList.Add(durationSeconds);
        psi.ArgumentList.Add("-c:v");
        psi.ArgumentList.Add("libx264");
        psi.ArgumentList.Add("-c:a");
        psi.ArgumentList.Add("aac");
        psi.ArgumentList.Add(outputPath);

        using var process = new Process { StartInfo = psi };

        if (!process.Start())
        {
            throw new InvalidOperationException("Failed to start ffmpeg process.");
        }

        // Дренируем stderr только в локальный буфер ограниченного размера, чтобы
        // огромный лог ffmpeg на больших архивах не утащил весь процесс в OOM
        // и не попадал целиком в Reason.
        var stdErrTask = ReadCappedAsync(process.StandardError, MaxReasonLength * 2, cancellationToken);
        var stdOutTask = process.StandardOutput.BaseStream.CopyToAsync(Stream.Null, cancellationToken);

        try
        {
            await process.WaitForExitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            try { process.Kill(entireProcessTree: true); }
            catch { /* процесс уже завершился */ }
            throw;
        }

        var stdErr = await stdErrTask;
        await stdOutTask;

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"ffmpeg failed with exit code {process.ExitCode}. StdErr: {stdErr}");
        }
    }

    private static async Task<string> ReadCappedAsync(StreamReader reader, int capChars, CancellationToken ct)
    {
        var buffer = new char[4096];
        var sb = new System.Text.StringBuilder();
        int read;
        while ((read = await reader.ReadAsync(buffer.AsMemory(0, buffer.Length), ct)) > 0)
        {
            if (sb.Length < capChars)
            {
                var toAppend = Math.Min(read, capChars - sb.Length);
                sb.Append(buffer, 0, toAppend);
            }
        }
        return sb.ToString();
    }
}
