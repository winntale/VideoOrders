using Dal.Abstractions.Enums;

namespace Dal.Abstractions.Entities;

public sealed class Resource
{
    public Guid Id { get; set; }
    public ResourceType Type { get; set; }
    public long TotalCapacity { get; set; }
    public long ReservedAmount { get; set; }
    public string Unit { get; set; } = null!;
}
