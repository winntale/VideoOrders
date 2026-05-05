namespace Dal.Abstractions.Entities;

public sealed record ArchiveFile
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }

    public string OriginalFileName { get; set; } = null!;
    public string StoredFileName { get; set; } = null!;
    public string StoragePath { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long FileSize { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public Order Order { get; set; } = null!;
}