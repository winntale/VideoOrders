namespace Events.Abstractions.Models;

public sealed record OrderCreatedEvent
{
    public required Guid OrderId { get; init; }
    public required Guid UserId { get; init; }
    public required Guid CameraId { get; init; }
    public required DateTimeOffset FromUtc { get; init; }
    public required DateTimeOffset ToUtc { get; init; }
    public required DateTimeOffset CreatedAtUtc { get; init; }
}