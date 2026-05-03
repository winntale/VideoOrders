using AutoMapper;
using Dal.Abstractions.Entities;
using Dal.Abstractions.Repositories;
using Events.Abstractions.Models;
using MassTransit;

namespace Gateway.Consumers;

public sealed class AuditOrderCompletedEventConsumer(
    IAuditRepository repository,
    IMapper mapper)
    : IConsumer<OrderCompletedEvent>
{
    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        var entry = mapper.Map<AuditEntry>(context.Message);
        await repository.AddAsync(entry, context.CancellationToken);
    }
}