using Dal.Abstractions.Enums;

namespace Core.Abstractions.OperationModels;

public sealed record ResourceEstimate
{
    public required ResourceType Type { get; init; }
    public required long Amount { get; init; }
    public required string Unit { get; init; }
}

public sealed record ResourceEstimateBundle
{
    public required IReadOnlyList<ResourceEstimate> Resources { get; init; }
    public required TimeSpan EstimatedProcessingDuration { get; init; }
}
