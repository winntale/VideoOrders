using Dal.Abstractions.Enums;
using Dal.Abstractions.Common;
using Dal.Abstractions.Models;
using Dal.Abstractions.Repositories;
using Events.Abstractions.Models;
using MassTransit;

namespace Gateway.Consumers;

public sealed class ResourceReservedEventConsumer(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork)
    : IConsumer<ResourceReservedEvent>
{
    public async Task Consume(ConsumeContext<ResourceReservedEvent> context)
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
            Status = OrderStatus.ResourceReserved,
            UpdatedAtUtc = message.ReservedAtUtc,
            FailureReason = null
        };

        await orderRepository.UpdateAsync(updatedOrder, context.CancellationToken);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}