using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Dal.Abstractions.Enums;

namespace Core.Operations;

public sealed class ResourceEstimator : IResourceEstimator
{
    // Размер выходного файла: ~8 МБ на 1 минуту архива (≈1.1 Мбит/с).
    private const double DiskMbPerMinute = 8.0;

    // На обработку (ffmpeg copy/trim/encode) выделяем небольшой запас сверху —
    // временный буфер, журналы, swap. 25% от итогового файла.
    private const double DiskOverheadFactor = 1.25;

    // Минимальный размер диска для самого короткого заказа.
    private const long DiskMinimumMb = 32;

    // ffmpeg с libx264 в типовой конфигурации использует ~2 ядра и ~256 МБ RAM
    // вне зависимости от длины видео. Для очень длинных заказов добавляем
    // небольшой запас по памяти под буфер чтения/записи.
    private const int CpuCoresPerJob = 2;
    private const long RamBaseMb = 256;
    private const long RamPerHourMb = 64;
    private const long RamMaxMb = 2048;

    // Время обработки на 2 ядрах при libx264 ≈ archive_duration / SpeedFactor.
    // Берём консервативное значение 4× (т.е. час архива ≈ 15 минут обработки)
    // плюс константный overhead на запуск, чтение метаданных и финализацию.
    private const double SpeedFactor = 4.0;
    private static readonly TimeSpan ProcessingOverhead = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan ProcessingMinimum = TimeSpan.FromSeconds(15);
    private static readonly TimeSpan ProcessingMaximum = TimeSpan.FromHours(6);

    public ResourceEstimateBundle Estimate(TimeSpan archiveDuration)
    {
        var minutes = Math.Max(0, archiveDuration.TotalMinutes);
        var hours = minutes / 60.0;

        var diskMb = Math.Max(
            DiskMinimumMb,
            (long)Math.Ceiling(minutes * DiskMbPerMinute * DiskOverheadFactor));

        var ramMb = Math.Min(
            RamMaxMb,
            RamBaseMb + (long)Math.Ceiling(hours * RamPerHourMb));

        var cpuCores = CpuCoresPerJob;

        var processingSeconds = archiveDuration.TotalSeconds / SpeedFactor;
        var processingDuration = TimeSpan.FromSeconds(processingSeconds) + ProcessingOverhead;
        if (processingDuration < ProcessingMinimum) processingDuration = ProcessingMinimum;
        if (processingDuration > ProcessingMaximum) processingDuration = ProcessingMaximum;

        return new ResourceEstimateBundle
        {
            Resources = new[]
            {
                new ResourceEstimate { Type = ResourceType.Cpu, Amount = cpuCores, Unit = "cores" },
                new ResourceEstimate { Type = ResourceType.Ram, Amount = ramMb, Unit = "MB" },
                new ResourceEstimate { Type = ResourceType.Disk, Amount = diskMb, Unit = "MB" },
            },
            EstimatedProcessingDuration = processingDuration,
        };
    }
}
