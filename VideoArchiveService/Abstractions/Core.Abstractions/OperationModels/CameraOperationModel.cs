namespace Core.Abstractions.OperationModels;

public sealed record CameraOperationModel
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required bool IsActive { get; init; }
    public IReadOnlyList<SegmentRangeOperationModel> Segments { get; init; } = Array.Empty<SegmentRangeOperationModel>();
}
