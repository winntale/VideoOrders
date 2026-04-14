using Dal.Abstractions.Enums;

namespace Dal.Abstractions.Entities;

public sealed record Order
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid CameraId { get; init; }
    public DateTimeOffset FromUtc { get; init; }
    public DateTimeOffset ToUtc { get; init; }
    public OrderStatus Status { get; init; }
    public string? FailureReason { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset? UpdatedAtUtc { get; init; }
}