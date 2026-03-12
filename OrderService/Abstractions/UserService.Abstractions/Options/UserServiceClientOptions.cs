namespace UserService.Abstractions.Options;

public sealed class UserServiceClientOptions
{
    public const string HttpClientName = "UserService";
    public string BaseUrl { get; init; } = string.Empty;
}