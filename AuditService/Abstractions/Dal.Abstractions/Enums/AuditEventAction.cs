namespace Dal.Abstractions.Enums;

public enum AuditEventAction
{
    Created = 0,
    Reserved = 1,
    ReservationFailed = 2,
    ProcessingStarted = 3,
    Completed = 4,
    Failed = 5
}