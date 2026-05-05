namespace Events.Abstractions.Models;

public sealed record OrderCompletedEvent
{
    public required Guid OrderId { get; init; }
    public required string OriginalFileName { get; init; } = null!;
    public required string StoredFileName { get; init; } = null!;
    public required string ContentType { get; init; } = null!;
    public required long FileSize { get; init; }
    public required DateTimeOffset CompletedAtUtc { get; init; }
}