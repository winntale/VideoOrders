namespace Dal.Abstractions.Models;

public sealed record ListOrdersByUserRepositoryModel
{
    public required Guid UserId { get; init; }
}
