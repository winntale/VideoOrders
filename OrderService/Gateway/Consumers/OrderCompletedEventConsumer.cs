using Core.Options;
using Dal.Abstractions.Enums;
using Dal.Abstractions.Common;
using Dal.Abstractions.Entities;
using Dal.Abstractions.Models;
using Dal.Abstractions.Repositories;
using Events.Abstractions.Models;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Gateway.Consumers;

public sealed class OrderCompletedEventConsumer(
    IOrderRepository orderRepository,
    IArchiveFileRepository archiveFileRepository,
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

        var fullPath = Path.Combine("../../archive_storage", message.StoredFileName).Replace('\\', '/');

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? string.Empty);

        var archiveFile = new ArchiveFile
        {
            Id = Guid.NewGuid(),
            OrderId = message.OrderId,
            OriginalFileName = message.OriginalFileName,
            StoredFileName = message.StoredFileName,
            StoragePath = fullPath,
            ContentType = message.ContentType,
            FileSize = message.FileSize,
            CreatedAtUtc = message.CompletedAtUtc
        };

        await orderRepository.UpdateAsync(updatedOrder, context.CancellationToken);
        await archiveFileRepository.AddAsync(archiveFile, context.CancellationToken);
        
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}