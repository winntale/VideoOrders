namespace Core.Abstractions.OperationModels;

public sealed record ArchiveAvailabilityResultOperationModel
{
    public required bool IsAvailable { get; init; }
    public required string? DenyReason { get; init; }
}