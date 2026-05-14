namespace VideoOrdersPortal.Models;

public sealed record CreateOrderDto
{
    public Guid CameraId { get; init; }
    public DateTime EventTime { get; init; }
    public int OffsetMinutes { get; init; } = 30;
}