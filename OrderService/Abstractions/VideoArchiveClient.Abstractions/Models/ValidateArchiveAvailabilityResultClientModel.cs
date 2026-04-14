namespace VideoArchiveClient.Abstractions.Models;

public sealed record ValidateArchiveAvailabilityResultClientModel
{
    public required bool IsAvailable { get; init; }
    public string? DenyReason { get; init; }
}