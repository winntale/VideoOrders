using AutoMapper;
using Dal.Abstractions.Entities;
using Dal.Abstractions.Repositories;
using Events.Abstractions.Models;
using MassTransit;

namespace Gateway.Consumers;

public sealed class AuditResourceReservationFailedEventConsumer(
    IAuditRepository repository,
    IMapper mapper)
    : IConsumer<ResourceReservationFailedEvent>
{
    public async Task Consume(ConsumeContext<ResourceReservationFailedEvent> context)
    {
        var entry = mapper.Map<AuditEntry>(context.Message);
        await repository.AddAsync(entry, context.CancellationToken);
    }
}