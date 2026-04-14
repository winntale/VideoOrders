namespace Core.Abstractions.OperationModels;

public sealed record UserAccessValidationResultOperationModel
{
    public required bool IsAllowed { get; init; }
    public string? DenyReason { get; init; }
}