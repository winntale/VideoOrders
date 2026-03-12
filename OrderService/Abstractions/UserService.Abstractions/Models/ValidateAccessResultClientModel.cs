namespace UserService.Abstractions.Models;

public sealed record ValidateAccessResultClientModel
{
    public required Guid UserId { get; init; }
    public required Guid CameraId { get; init; }
    public required bool IsAllowed { get; init; }
    public string? DenyReason { get; init; }
    public bool UserNotFound { get; init; }
}