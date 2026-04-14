namespace Gateway.Models;

public sealed record UserAccessValidationResponseDto
{
    public required bool IsAllowed { get; init; }
    public string? DenyReason { get; init; }
}