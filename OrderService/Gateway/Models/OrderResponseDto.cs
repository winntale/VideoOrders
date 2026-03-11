namespace Gateway.Models;

public sealed record OrderResponseDto
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required Guid CameraId { get; init; }
    public required DateTimeOffset FromUtc { get; init; }
    public required DateTimeOffset ToUtc { get; init; }
    public required string Status { get; init; }
    public string? FailureReason { get; init; }
}