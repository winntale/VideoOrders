namespace Events.Abstractions.Models;

public sealed record ResourceReservedEvent
{
    public required Guid OrderId { get; init; }
    public required Guid CameraId { get; init; }
    public required DateTimeOffset FromUtc { get; init; }
    public required DateTimeOffset ToUtc { get; init; }
    public required DateTimeOffset ReservedAtUtc { get; init; }
    public DateTimeOffset? SegmentStartUtc { get; init; }
}