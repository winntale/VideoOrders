using Dal.Abstractions.Common;
using Dal.Abstractions.Enums;
using Dal.Abstractions.Models;
using Dal.Abstractions.Repositories;
using Events.Abstractions.Models;
using MassTransit;

namespace Gateway.Consumers;

public class ProcessingStartedEventConsumer(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork)
    : IConsumer<ProcessingStartedEvent>
{
    public async Task Consume(ConsumeContext<ProcessingStartedEvent> context)
    {
        var message = context.Message;

        var order = await orderRepository.GetByIdAsync(
            new GetOrderByIdRepositoryModel { Id = message.OrderId },
            context.CancellationToken);

        if (order is null)
        {
            return;
        }

        if (order.Status != OrderStatus.ResourceReserved)
        {
            return;
        }
        
        Console.WriteLine($"[OrderService] ProcessingStarted received. OrderId: {message.OrderId}");

        var updatedOrder = order with
        {
            Status = OrderStatus.ProcessingStarted,
            UpdatedAtUtc = message.StartedAtUtc,
            FailureReason = null
        };

        await orderRepository.UpdateAsync(updatedOrder, context.CancellationToken);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
        
        Console.WriteLine($"[OrderService] Status updated. OrderId: {message.OrderId}");
    }
}