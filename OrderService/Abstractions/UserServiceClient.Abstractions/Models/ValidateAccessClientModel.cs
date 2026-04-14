namespace UserServiceClient.Abstractions.Models;

public sealed record ValidateAccessClientModel
{
    public required Guid UserId { get; init; }
    public required Guid CameraId { get; init; }
}