using Core.Abstractions.Enums;

namespace Core.Abstractions.OperationModels;

public sealed record ChangeOrderStatusOperationModel
{
    public required Guid Id { get; init; }
    public required OrderStatus Status { get; init; }
}