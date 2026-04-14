namespace UserServiceClient.Abstractions.Models;

public sealed record ValidateAccessResultClientModel
{
    public required bool IsAllowed { get; init; }
    public required string? DenyReason { get; init; }
}