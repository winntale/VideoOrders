namespace Dal.ElasticModels;

public sealed record AuditEntryDocument
{
    public Guid Id { get; init; }
    public Guid? OrderId { get; init; }
    public Guid? UserId { get; init; }
    public Guid? CameraId { get; init; }

    public string ServiceName { get; init; } = null!;
    public string EventType { get; init; } = null!;
    public string EventAction { get; init; } = null!;
    public string EventOutcome { get; init; } = null!;
    public string? StatusFrom { get; init; }
    public string? StatusTo { get; init; }
    public string? FailureReason { get; init; }

    public string PayloadJson { get; init; } = null!;
    public DateTimeOffset OccurredAtUtc { get; init; }
}