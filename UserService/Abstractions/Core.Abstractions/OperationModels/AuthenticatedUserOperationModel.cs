namespace Core.Abstractions.OperationModels;

public sealed record AuthenticatedUserOperationModel
{
    public required Guid UserId { get; init; }
    public required string Login { get; init; }
}
