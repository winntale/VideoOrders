namespace Events.Abstractions.Models;

public sealed record ResourceReservedEvent
{
    public required Guid OrderId { get; init; }
    public required DateTimeOffset ReservedAtUtc { get; init; }
}