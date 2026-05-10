namespace Gateway.Models;

public sealed record CameraDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required bool IsActive { get; init; }
}
