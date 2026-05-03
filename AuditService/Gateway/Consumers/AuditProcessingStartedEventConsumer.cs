using AutoMapper;
using Dal.Abstractions.Entities;
using Dal.Abstractions.Repositories;
using Events.Abstractions.Models;
using MassTransit;

namespace Gateway.Consumers;

public sealed class AuditProcessingStartedEventConsumer(
    IAuditRepository repository,
    IMapper mapper)
    : IConsumer<ProcessingStartedEvent>
{
    public async Task Consume(ConsumeContext<ProcessingStartedEvent> context)
    {
        var entry = mapper.Map<AuditEntry>(context.Message);
        await repository.AddAsync(entry, context.CancellationToken);
    }
}