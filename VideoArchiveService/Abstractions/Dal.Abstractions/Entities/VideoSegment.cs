namespace Dal.Abstractions.Entities;

public sealed record VideoSegment
{
    public Guid Id { get; init; }
    public Guid CameraId { get; init; }
    public required DateTimeOffset FromUtc { get; init; }
    public required DateTimeOffset ToUtc { get; init; }
    public Camera? Camera { get; init; }
}