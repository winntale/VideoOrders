namespace Dal.Abstractions.Enums;

public enum AuditEventType
{
    OrderCreatedEvent = 0,
    ResourceReservedEvent = 1,
    ResourceReservationFailedEvent = 2,
    ProcessingStartedEvent = 3,
    OrderCompletedEvent = 4,
    OrderFailedEvent = 5
}