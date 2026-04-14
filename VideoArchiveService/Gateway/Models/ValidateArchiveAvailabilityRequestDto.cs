namespace Gateway.Models;

public sealed record ValidateArchiveAvailabilityRequestDto
{
    public required Guid CameraId { get; init; }
    public required DateTimeOffset FromUtc { get; init; }
    public required DateTimeOffset ToUtc { get; init; }
}