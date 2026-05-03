using System.Text.Json;
using AutoMapper;
using Dal.Abstractions.Entities;
using Dal.Abstractions.Repositories;
using Events.Abstractions.Models;
using MassTransit;

namespace Gateway.Consumers;

public sealed class AuditResourceReservedEventConsumer(
    IAuditRepository repository,
    IMapper mapper)
: IConsumer<ResourceReservedEvent>
{
    public async Task Consume(ConsumeContext<ResourceReservedEvent> context)
    {
        var entry = mapper.Map<AuditEntry>(context.Message);
        await repository.AddAsync(entry, context.CancellationToken);
    }
}