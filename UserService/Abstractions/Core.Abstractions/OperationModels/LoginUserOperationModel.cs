namespace Core.Abstractions.OperationModels;

public sealed record LoginUserOperationModel
{
    public required string Login { get; init; }
    public required string Password { get; init; }
}
