using Dal.Abstractions.Enums;
using Dal.Abstractions.Common;
using Dal.Abstractions.Models;
using Dal.Abstractions.Repositories;
using Events.Abstractions.Models;
using MassTransit;

namespace Gateway.Consumers;

public sealed class OrderCompletedEventConsumer(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork)
    : IConsumer<OrderCompletedEvent>
{
    public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
    {
        var message = context.Message;

        var order = await orderRepository.GetByIdAsync(
            new GetOrderByIdRepositoryModel { Id = message.OrderId },
            context.CancellationToken);

        if (order is null)
        {
            return;
        }

        if (order.Status is not (OrderStatus.ProcessingStarted or OrderStatus.ResourceReserved))
        {
            return;
        }

        var updatedOrder = order with
        {
            Status = OrderStatus.Completed,
            UpdatedAtUtc = message.CompletedAtUtc,
            FailureReason = null
        };

        await orderRepository.UpdateAsync(updatedOrder, context.CancellationToken);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}