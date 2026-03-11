namespace Gateway.Models;

public sealed record UserAccessValidationResponseDto
{
    public required Guid UserId { get; init; }
    public required Guid CameraId { get; init; }
    public required bool IsAllowed { get; init; }
    public string? DenyReason { get; init; }
}