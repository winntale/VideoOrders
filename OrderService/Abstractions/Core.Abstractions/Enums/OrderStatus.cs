namespace Core.Abstractions.Enums;

public enum OrderStatus
{
    Created = 0,
    ResourceReserved = 1,
    ResourceReservationFailed = 2,
    ProcessingStarted = 3,
    Completed = 4,
    Failed = 5
}