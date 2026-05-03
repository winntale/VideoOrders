using AutoMapper;
using Dal.Abstractions.Entities;
using Dal.Abstractions.Repositories;
using Events.Abstractions.Models;
using MassTransit;

namespace Gateway.Consumers;

public sealed class AuditOrderFailedEventConsumer(
    IAuditRepository repository,
    IMapper mapper)
    : IConsumer<OrderFailedEvent>
{
    public async Task Consume(ConsumeContext<OrderFailedEvent> context)
    {
        var entry = mapper.Map<AuditEntry>(context.Message);
        await repository.AddAsync(entry, context.CancellationToken);
    }
}