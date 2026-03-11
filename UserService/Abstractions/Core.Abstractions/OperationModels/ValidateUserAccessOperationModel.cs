namespace Core.Abstractions.OperationModels;

public sealed record ValidateUserAccessOperationModel
{
    public required Guid UserId { get; init; }
    public required Guid CameraId { get; init; }
    public required DateTimeOffset FromUtc { get; init; }
    public required DateTimeOffset ToUtc { get; init; }
}