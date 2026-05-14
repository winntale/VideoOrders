using Core.Abstractions.Operations;
using Events.Abstractions.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gateway.Consumers;

public sealed class OrderCreatedConsumer(
    IReserveResourcesOperation reserveResources,
    ILogger<OrderCreatedConsumer> logger)
    : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;

        logger.LogInformation(
            "Получен OrderCreated: OrderId={OrderId}, длительность={Duration}",
            message.OrderId,
            message.ToUtc - message.FromUtc);

        var outcome = await reserveResources.ExecuteAsync(
            message.OrderId,
            message.FromUtc,
            message.ToUtc,
            context.CancellationToken);

        if (outcome.Success)
        {
            logger.LogInformation(
                "Ресурсы зарезервированы для заказа {OrderId}: {Estimates}",
                message.OrderId,
                string.Join(", ", outcome.Estimates.Select(e => $"{e.Type}={e.Amount}{e.Unit}")));

            await context.Publish(new ResourceReservedEvent
            {
                OrderId = message.OrderId,
                CameraId = message.CameraId,
                FromUtc = message.FromUtc,
                ToUtc = message.ToUtc,
                ReservedAtUtc = DateTimeOffset.UtcNow,
                SegmentStartUtc = message.SegmentStartUtc,
            });
        }
        else
        {
            logger.LogWarning(
                "Не удалось зарезервировать ресурсы для заказа {OrderId}: {Reason}",
                message.OrderId,
                outcome.FailureReason);

            await context.Publish(new ResourceReservationFailedEvent
            {
                OrderId = message.OrderId,
                Reason = outcome.FailureReason ?? "Resources are unavailable.",
                FailedAtUtc = DateTimeOffset.UtcNow,
            });
        }
    }
}
