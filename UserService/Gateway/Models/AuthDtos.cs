namespace Gateway.Models;

public sealed record LoginRequestDto
{
    public required string Login { get; init; }
    public required string Password { get; init; }
}

public sealed record RegisterRequestDto
{
    public required string Login { get; init; }
    public required string Password { get; init; }
}

public sealed record AuthenticatedUserDto
{
    public required Guid UserId { get; init; }
    public required string Login { get; init; }
}
