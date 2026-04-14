namespace UserServiceClient.Abstractions.Options;

public sealed record UserServiceClientOptions
{
    public string BaseUrl { get; init; }
}