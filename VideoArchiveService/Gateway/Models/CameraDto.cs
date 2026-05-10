namespace Gateway.Models;

public sealed record SegmentRangeDto
{
    public required DateTimeOffset FromUtc { get; init; }
    public required DateTimeOffset ToUtc { get; init; }
}

public sealed record CameraDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required bool IsActive { get; init; }
    public IReadOnlyList<SegmentRangeDto> Segments { get; init; } = Array.Empty<SegmentRangeDto>();
}
