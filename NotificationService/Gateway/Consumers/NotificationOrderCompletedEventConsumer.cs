using Dal.Abstractions.Entities;
using Dal.Abstractions.Enums;
using Dal.Abstractions.Repositories;
using Events.Abstractions.Models;
using MassTransit;

namespace Gateway.Consumers;

public class NotificationOrderCompletedEventConsumer(INotificationRepository repository)
    : IConsumer<OrderCompletedEvent>
{
    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        var message = context.Message;

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            OrderId = message.OrderId,
            Type = NotificationType.OrderCompleted,
            Message = $"Order '{message.OrderId}' completed successfully.",
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        await repository.AddAsync(notification, context.CancellationToken);
    }
}