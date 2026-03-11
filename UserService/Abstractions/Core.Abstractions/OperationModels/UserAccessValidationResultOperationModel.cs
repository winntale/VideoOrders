namespace Core.Abstractions.OperationModels;

public sealed record UserAccessValidationResultOperationModel
{
    public required Guid UserId { get; init; }
    public required Guid CameraId { get; init; }
    public required bool IsAllowed { get; init; }
    public string? DenyReason { get; init; }
}