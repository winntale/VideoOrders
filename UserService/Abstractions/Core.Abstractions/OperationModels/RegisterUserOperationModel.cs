namespace Core.Abstractions.OperationModels;

public sealed record RegisterUserOperationModel
{
    public required string Login { get; init; }
    public required string Password { get; init; }
}
