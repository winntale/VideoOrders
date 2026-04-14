namespace Dal.Abstractions.Models;

public sealed record CameraRepositoryModel
{
    public required Guid CameraId { get; init; }
}