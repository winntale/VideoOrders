namespace Core.Abstractions.OperationModels;

public sealed record FileDownloadOperationResult
{
    public Stream Stream { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public string FileName { get; init; } = null!;
    public long FileSize { get; init; }
}