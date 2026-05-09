namespace Core.Abstractions.OperationModels;

public sealed record ListOrdersByUserOperationModel
{
    public required Guid UserId { get; init; }
}
