using Dal.Abstractions.Enums;

namespace Dal.Abstractions.Entities;

public sealed record Order
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid CameraId { get; set; }
    public DateTimeOffset FromUtc { get; set; }
    public DateTimeOffset ToUtc { get; set; }
    public OrderStatus Status { get; set; }
    public string? FailureReason { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset? UpdatedAtUtc { get; set; }
}