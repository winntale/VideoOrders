using Dal.Abstractions.Common;
using Dal.Abstractions.Enums;
using Dal.Abstractions.Models;
using Dal.Abstractions.Repositories;
using Events.Abstractions.Models;
using MassTransit;

namespace Gateway.Consumers;

public sealed class ResourceReservationFailedEventConsumer(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork)
    : IConsumer<ResourceReservationFailedEvent>
{
    public async Task Consume(ConsumeContext<ResourceReservationFailedEvent> context)
    {
        var message = context.Message;

        var order = await orderRepository.GetByIdAsync(
            new GetOrderByIdRepositoryModel
            {
                Id = message.OrderId
            },
            context.CancellationToken);

        if (order is null)
        {
            return;
        }
        
        if (order.Status != OrderStatus.Created)
        {
            return;
        }

        var updatedOrder = order with
        {
            Status = OrderStatus.ResourceReservationFailed,
            UpdatedAtUtc = message.FailedAtUtc,
            FailureReason = message.Reason
        };

        await orderRepository.UpdateAsync(updatedOrder, context.CancellationToken);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}