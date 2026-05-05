namespace Gateway.Options;

public sealed record ArchiveStorageOptions
{
    public string RootPath { get; init; } = null!;
}