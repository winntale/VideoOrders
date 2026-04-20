using Dal.Abstractions.Common;
using Dal.Abstractions.Enums;
using Dal.Abstractions.Models;
using Dal.Abstractions.Repositories;
using Events.Abstractions.Models;
using MassTransit;

namespace Gateway.Consumers;

public sealed class OrderFailedEventConsumer(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork)
    : IConsumer<OrderFailedEvent>
{
    public async Task Consume(ConsumeContext<OrderFailedEvent> context)
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
            Status = OrderStatus.Failed,
            UpdatedAtUtc = message.FailedAtUtc,
            FailureReason = message.Reason
        };

        await orderRepository.UpdateAsync(updatedOrder, context.CancellationToken);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}