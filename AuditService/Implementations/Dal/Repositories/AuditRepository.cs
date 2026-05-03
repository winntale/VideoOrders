using AutoMapper;
using Dal.Abstractions.Entities;
using Dal.Abstractions.Enums;
using Dal.Abstractions.Repositories;
using Dal.Clients;
using Dal.ElasticModels;

namespace Dal.Repositories;

internal sealed class AuditRepository(
    AuditElasticsearchClient elasticClient,
    IMapper mapper) : IAuditRepository
{
    public async Task AddAsync(AuditEntry entry, CancellationToken cancellationToken)
    {
        var document = mapper.Map<AuditEntryDocument>(entry);

        await elasticClient.Client.IndexAsync(
            document,
            idx => idx.Index(elasticClient.IndexName).Id(document.Id),
            cancellationToken);
    }

    public async Task<IReadOnlyList<AuditEntry>> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var response = await elasticClient.Client.SearchAsync<AuditEntryDocument>(
            s => s
                .Index(elasticClient.IndexName)
                .Query(q => q.Term(t => t.Field(f => f.OrderId).Value(orderId.ToString())))
                .Sort(so => so.Field(f => f.OccurredAtUtc)),
            cancellationToken);

        return response.Documents
            .Select(doc => new AuditEntry
            {
                Id = doc.Id,
                OrderId = doc.OrderId,
                UserId = doc.UserId,
                CameraId = doc.CameraId,
                ServiceName = Enum.Parse<AuditServiceName>(doc.ServiceName),
                EventType = Enum.Parse<AuditEventType>(doc.EventType),
                EventAction = Enum.Parse<AuditEventAction>(doc.EventAction),
                EventOutcome = Enum.Parse<AuditEventOutcome>(doc.EventOutcome),
                StatusFrom = doc.StatusFrom is null ? null : Enum.Parse<OrderLifecycleStatus>(doc.StatusFrom),
                StatusTo = doc.StatusTo is null ? null : Enum.Parse<OrderLifecycleStatus>(doc.StatusTo),
                FailureReason = doc.FailureReason,
                PayloadJson = doc.PayloadJson,
                OccurredAtUtc = doc.OccurredAtUtc
            })
            .ToList();
    }
}