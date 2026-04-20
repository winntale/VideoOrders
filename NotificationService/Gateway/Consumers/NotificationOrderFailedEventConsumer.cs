using Dal.Abstractions.Entities;
using Dal.Abstractions.Enums;
using Dal.Abstractions.Repositories;
using Events.Abstractions.Models;
using MassTransit;

namespace Gateway.Consumers;

public sealed class NotificationOrderFailedEventConsumer(INotificationRepository repository)
    : IConsumer<OrderFailedEvent>
{
    public async Task Consume(ConsumeContext<OrderFailedEvent> context)
    {
        var message = context.Message;

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            OrderId = message.OrderId,
            Type = NotificationType.OrderFailed,
            Message = $"Order '{message.OrderId}' failed: '{message.Reason}'",
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        await repository.AddAsync(notification, context.CancellationToken);
    }
}