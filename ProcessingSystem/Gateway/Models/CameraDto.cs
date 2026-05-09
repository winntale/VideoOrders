namespace Gateway.Models;

public sealed record CameraDto
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
    public required long FileSize { get; init; }
}
