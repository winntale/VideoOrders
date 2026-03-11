namespace Dal.Abstractions.Entities;

public sealed record UserCameraAccess
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CameraId { get; set; }
}