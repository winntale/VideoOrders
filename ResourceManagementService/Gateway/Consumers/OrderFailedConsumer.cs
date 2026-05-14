using Core.Abstractions.Operations;
using Events.Abstractions.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gateway.Consumers;

public sealed class OrderFailedConsumer(
    IReleaseResourcesOperation release,
    ILogger<OrderFailedConsumer> logger)
    : IConsumer<OrderFailedEvent>
{
    public async Task Consume(ConsumeContext<OrderFailedEvent> context)
    {
        var released = await release.ExecuteAsync(context.Message.OrderId, context.CancellationToken);
        logger.LogInformation(
            "Освобождено {Count} резерваций по OrderFailed: OrderId={OrderId}, причина={Reason}",
            released, context.Message.OrderId, context.Message.Reason);
    }
}
