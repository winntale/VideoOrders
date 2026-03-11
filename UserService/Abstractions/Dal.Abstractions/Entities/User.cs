using Dal.Abstractions.Enums;

namespace Dal.Abstractions.Entities;

public sealed record User
{
    public Guid Id { get; set; }
    public string Login { get; set; } = null!;
    public UserStatus Status { get; set; }
}