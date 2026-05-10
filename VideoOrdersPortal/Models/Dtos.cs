namespace VideoOrdersPortal.Models;

public sealed record SegmentRange(DateTimeOffset FromUtc, DateTimeOffset ToUtc);

public sealed record CameraDto(
    Guid Id,
    string Name,
    long FileSize,
    IReadOnlyList<SegmentRange> Segments);

public sealed record UserNotificationDto(
    Guid Id,
    Guid OrderId,
    Guid CameraId,
    string CameraName,
    string Type,
    string Message,
    DateTimeOffset CreatedAtUtc);

public sealed record OrderListItemDto(
    Guid Id,
    Guid UserId,
    Guid CameraId,
    string CameraName,
    DateTimeOffset FromUtc,
    DateTimeOffset ToUtc,
    string Status,
    string? FailureReason,
    ArchiveFileDto? ArchiveFile);

public sealed record ArchiveFileDto(
    string OriginalFileName,
    string ContentType,
    long FileSize,
    bool IsReady,
    string DownloadUrl,
    string StreamUrl);

public sealed record OrderDto(
    Guid Id,
    Guid UserId,
    Guid CameraId,
    DateTimeOffset FromUtc,
    DateTimeOffset ToUtc,
    string Status,
    string? FailureReason,
    ArchiveFileDto? ArchiveFile);

public sealed record CreateOrderRequest(
    Guid CameraId,
    DateTimeOffset FromUtc,
    DateTimeOffset ToUtc);

public sealed record ValidateAccessRequest(Guid CameraId);

public sealed record ValidateArchiveRequest(
    Guid CameraId,
    DateTimeOffset FromUtc,
    DateTimeOffset ToUtc);

public sealed record ValidationResult(bool IsAllowed, string? DenyReason);
