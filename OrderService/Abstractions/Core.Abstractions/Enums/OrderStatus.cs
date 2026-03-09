namespace Core.Abstractions.Enums;

public enum OrderStatus
{
    Created = 0,
    Validating = 1,
    ValidationFailed = 2,
    Processing = 3,
    Completed = 4,
    Failed = 5
}