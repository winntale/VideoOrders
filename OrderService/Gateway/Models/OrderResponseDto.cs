namespace Gateway.Models;

public sealed record OrderResponseDto
{
    public required Guid Id { get; init; }
    public required string Status { get; init; }
    public string? FailureReason { get; init; }
}