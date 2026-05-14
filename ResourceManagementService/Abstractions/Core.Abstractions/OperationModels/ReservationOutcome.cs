namespace Core.Abstractions.OperationModels;

public sealed record ReservationOutcome
{
    public required bool Success { get; init; }
    public required IReadOnlyList<ResourceEstimate> Estimates { get; init; }
    public TimeSpan EstimatedProcessingDuration { get; init; }
    public DateTimeOffset? HoldUntilUtc { get; init; }
    public string? FailureReason { get; init; }
}
