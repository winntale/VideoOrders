namespace Gateway.Models;

public sealed record ArchiveFileDto
{
    public string OriginalFileName { get; init; } = null!;
    public string ContentType { get; init; } = null!;
    public long FileSize { get; init; }
    public bool IsReady { get; init; }
    public string DownloadUrl { get; init; } = null!;
    public string StreamUrl { get; init; } = null!;
}