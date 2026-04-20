using Dal.Abstractions.Entities;
using Dal.Abstractions.Enums;
using Dal.Abstractions.Repositories;
using Events.Abstractions.Models;
using MassTransit;

namespace Gateway.Consumers;

public sealed class NotificationResourceReservationFailedEventConsumer(INotificationRepository repository)
    : IConsumer<ResourceReservationFailedEvent>
{
    public async Task Consume(ConsumeContext<ResourceReservationFailedEvent> context)
    {
        var message = context.Message;

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            OrderId = message.OrderId,
            Type = NotificationType.ResourceReservationFailed,
            Message = $"Order '{message.OrderId}' failed at resource reservation: '{message.Reason}'",
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        await repository.AddAsync(notification, context.CancellationToken);
    }
}