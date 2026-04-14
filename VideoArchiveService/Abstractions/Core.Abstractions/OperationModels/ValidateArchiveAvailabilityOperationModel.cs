namespace Core.Abstractions.OperationModels;

public sealed record ValidateArchiveAvailabilityOperationModel
{
    public required Guid CameraId { get; init; }
    public required DateTimeOffset FromUtc { get; init; }
    public required DateTimeOffset ToUtc { get; init; }
}