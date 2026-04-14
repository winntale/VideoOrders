namespace VideoArchiveClient.Abstractions.Options;

public sealed record VideoArchiveServiceClientOptions
{
    public string BaseUrl { get; init; }
}