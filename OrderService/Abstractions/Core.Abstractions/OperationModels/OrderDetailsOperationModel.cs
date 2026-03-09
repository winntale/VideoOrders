using Core.Abstractions.Enums;

namespace Core.Abstractions.OperationModels;

public sealed record OrderDetailsOperationModel
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required Guid CameraId { get; init; }
    public required DateTimeOffset FromUtc { get; init; }
    public required DateTimeOffset ToUtc { get; init; }
    public required OrderStatus Status { get; init; }
    public required string? FailureReason { get; init; }
}