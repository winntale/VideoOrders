namespace Events.Abstractions.Models;

public sealed record OrderCompletedEvent
{
    public required Guid OrderId { get; init; }
    public required DateTimeOffset CompletedAtUtc { get; init; }
}