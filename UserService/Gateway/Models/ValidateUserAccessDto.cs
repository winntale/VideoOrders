namespace Gateway.Models;

public sealed record ValidateUserAccessDto
{
    public required Guid UserId { get; init; }
    public required Guid CameraId { get; init; }
    public required DateTimeOffset FromUtc { get; init; }
    public required DateTimeOffset ToUtc { get; init; }
}