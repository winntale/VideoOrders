using Core.Abstractions.Operations;
using Events.Abstractions.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gateway.Consumers;

public sealed class OrderCompletedConsumer(
    IReleaseResourcesOperation release,
    ILogger<OrderCompletedConsumer> logger)
    : IConsumer<OrderCompletedEvent>
{
    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        var released = await release.ExecuteAsync(context.Message.OrderId, context.CancellationToken);
        logger.LogInformation(
            "Освобождено {Count} резерваций по OrderCompleted: OrderId={OrderId}",
            released, context.Message.OrderId);
    }
}
