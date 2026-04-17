namespace Events.Abstractions.Models;

public sealed record ProcessingStartedEvent
{
    public required Guid OrderId { get; init; }
    public required DateTimeOffset StartedAtUtc { get; init; }
}