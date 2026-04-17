using Events.Abstractions.Models;
using MassTransit;

namespace Gateway.Consumers;

public sealed class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;

        var resourcesAvailable = true;

        if (resourcesAvailable)
        {
            await context.Publish(
                new ResourceReservedEvent
                {
                    OrderId = message.OrderId,
                    ReservedAtUtc = DateTimeOffset.UtcNow
                });
        }
        else
        {
            await context.Publish(
                new ResourceReservationFailedEvent
                {
                    OrderId = message.OrderId,
                    Reason = "Resources are unavailable.",
                    FailedAtUtc = DateTimeOffset.UtcNow
                });
        }
    }
}