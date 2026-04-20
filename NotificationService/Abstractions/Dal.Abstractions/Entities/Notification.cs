using Dal.Abstractions.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dal.Abstractions.Entities;

public sealed record Notification
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; init; }
    
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid OrderId { get; init; }
    public NotificationType Type { get; init; }
    public string Message { get; init; } = null!;
    public DateTimeOffset CreatedAtUtc { get; init; }
}