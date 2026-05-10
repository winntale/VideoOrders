using Dal.Abstractions.Entities;
using Dal.Abstractions.Repositories;
using Dal.Context;
using MongoDB.Driver;

namespace Dal.Repositories;

internal sealed class NotificationRepository(NotificationDbContext dbContext) : INotificationRepository
{
    public Task AddAsync(Notification notification, CancellationToken cancellationToken)
    {
        return dbContext.Notifications.InsertOneAsync(
            notification,
            null,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await dbContext.Notifications
            .Find(x => x.OrderId == orderId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Notification>> GetByOrderIdsAsync(
        IReadOnlyCollection<Guid> orderIds,
        CancellationToken cancellationToken)
    {
        if (orderIds.Count == 0)
        {
            return Array.Empty<Notification>();
        }

        var filter = Builders<Notification>.Filter.In(x => x.OrderId, orderIds);
        return await dbContext.Notifications
            .Find(filter)
            .SortByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}