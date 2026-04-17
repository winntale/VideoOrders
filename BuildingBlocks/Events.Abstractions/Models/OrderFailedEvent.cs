namespace Events.Abstractions.Models;

public sealed record OrderFailedEvent
{
    public required Guid OrderId { get; init; }
    public required string Reason { get; init; }
    public required DateTimeOffset FailedAtUtc { get; init; }
}