namespace Dal.Abstractions.Models;

public sealed record VideoSegmentRepositoryModel
{
    public required Guid CameraId { get; init; }
    public required DateTimeOffset FromUtc { get; init; }
    public required DateTimeOffset ToUtc { get; init; }
}