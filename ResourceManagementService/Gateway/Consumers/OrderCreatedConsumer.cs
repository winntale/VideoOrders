using Events.Abstractions.Models;
using MassTransit;

namespace Gateway.Consumers;

public sealed class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        Console.WriteLine($"Получен OrderCreated: {context.Message.OrderId}");

        var message = context.Message;

        var resourcesAvailable = true;

        if (resourcesAvailable)
        {
            await context.Publish(
                new ResourceReservedEvent
                {
                    OrderId = message.OrderId,
                    CameraId = message.CameraId,
                    FromUtc = message.FromUtc,
                    ToUtc = message.ToUtc,
                    ReservedAtUtc = DateTimeOffset.UtcNow,
                    SegmentStartUtc = message.SegmentStartUtc
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