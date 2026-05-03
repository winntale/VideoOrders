using Dal.Abstractions.Enums;

namespace Dal.Abstractions.Entities;

public sealed record AuditEntry
{
    public Guid Id { get; init; }
    public Guid? OrderId { get; init; }
    public Guid? UserId { get; init; }
    public Guid? CameraId { get; init; }

    public AuditServiceName ServiceName { get; init; }
    public AuditEventType EventType { get; init; }
    public AuditEventAction EventAction { get; init; }
    public AuditEventOutcome EventOutcome { get; init; }

    public OrderLifecycleStatus? StatusFrom { get; init; }
    public OrderLifecycleStatus? StatusTo { get; init; }
    public string? FailureReason { get; init; }

    public string PayloadJson { get; init; } = null!;
    public DateTimeOffset OccurredAtUtc { get; init; }
}