namespace Dal.Abstractions.Models;

public sealed record GetOrderByIdRepositoryModel
{
    public required Guid Id { get; init; }
}