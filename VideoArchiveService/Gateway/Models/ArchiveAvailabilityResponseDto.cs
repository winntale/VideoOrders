namespace Gateway.Models;

public sealed record ArchiveAvailabilityResponseDto
{
    public required bool IsAvailable { get; init; }
    public required string? DenyReason { get; init; }
}