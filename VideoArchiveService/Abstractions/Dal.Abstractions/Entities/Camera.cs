namespace Dal.Abstractions.Entities;

public sealed record Camera
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public bool IsActive { get; init; }
    public IReadOnlyCollection<VideoSegment> VideoSegments { get; init; } = [];
}