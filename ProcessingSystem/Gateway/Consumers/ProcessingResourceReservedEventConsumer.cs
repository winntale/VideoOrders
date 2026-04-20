using Events.Abstractions.Models;
using MassTransit;

namespace Gateway.Consumers;

public sealed class ProcessingResourceReservedEventConsumer : IConsumer<ResourceReservedEvent>
{
    public async Task Consume(ConsumeContext<ResourceReservedEvent> context)
    {
        var message = context.Message;
        
        Console.WriteLine($"[ProcessingSystem] ResourceReserved received. OrderId: {message.OrderId}");

        await context.Publish(
            new ProcessingStartedEvent
            {
                OrderId = message.OrderId,
                StartedAtUtc = DateTimeOffset.UtcNow
            });

        await Task.Delay(TimeSpan.FromSeconds(2), context.CancellationToken);

        var isSuccess = message.OrderId.GetHashCode() % 5 != 0;

        if (isSuccess)
        {
            Console.WriteLine($"[ProcessingSystem] Publishing OrderCompletedEvent. OrderId: {message.OrderId}");
            
            await context.Publish(
                new OrderCompletedEvent
                {
                    OrderId = message.OrderId,
                    CompletedAtUtc = DateTimeOffset.UtcNow
                });
        }
        else
        {
            await context.Publish(
                new OrderFailedEvent
                {
                    OrderId = message.OrderId,
                    Reason = "Processing failed due to internal pipeline error.",
                    FailedAtUtc = DateTimeOffset.UtcNow
                });
        }
    }
}