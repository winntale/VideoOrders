namespace VideoArchiveClient.Abstractions.Models;

public sealed record ValidateArchiveAvailabilityClientModel
{
    public required Guid CameraId { get; init; }
    public required DateTimeOffset FromUtc { get; init; }
    public required DateTimeOffset ToUtc { get; init; }
}