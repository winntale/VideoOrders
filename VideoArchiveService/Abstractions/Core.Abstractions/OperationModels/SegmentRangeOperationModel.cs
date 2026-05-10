namespace Core.Abstractions.OperationModels;

public sealed record SegmentRangeOperationModel
{
    public required DateTimeOffset FromUtc { get; init; }
    public required DateTimeOffset ToUtc { get; init; }
}
