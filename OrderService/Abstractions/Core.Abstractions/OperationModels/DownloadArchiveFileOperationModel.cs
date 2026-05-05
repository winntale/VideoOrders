namespace Core.Abstractions.OperationModels;

public sealed record DownloadArchiveFileOperationModel
{
    public Guid OrderId { get; init; }
}