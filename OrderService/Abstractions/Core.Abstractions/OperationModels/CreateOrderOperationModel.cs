namespace Core.Abstractions.OperationModels;

public sealed record CreateOrderOperationModel
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required Guid CameraId { get; init; }
    public required DateTimeOffset FromUtc { get; init; }
    public required DateTimeOffset ToUtc { get; init; }
}