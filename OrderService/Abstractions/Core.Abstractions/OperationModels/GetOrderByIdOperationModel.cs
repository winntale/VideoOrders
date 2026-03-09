namespace Core.Abstractions.OperationModels;

public sealed record GetOrderByIdOperationModel
{
    public required Guid Id { get; init; }
}