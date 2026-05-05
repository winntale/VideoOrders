namespace Core.Abstractions.OperationModels;

public sealed record StreamArchiveFileOperationModel
{
    public Guid OrderId { get; init; }
}