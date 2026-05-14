using Dal.Abstractions.Enums;

namespace Dal.Abstractions.Entities;

public sealed class Reservation
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public ResourceType ResourceType { get; set; }
    public long Amount { get; set; }
    public DateTimeOffset ReservedAtUtc { get; set; }
    public DateTimeOffset HoldUntilUtc { get; set; }
    public DateTimeOffset? ReleasedAtUtc { get; set; }
    public ReservationStatus Status { get; set; }
}
