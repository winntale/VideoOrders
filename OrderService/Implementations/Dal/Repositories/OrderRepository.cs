using Dal.Abstractions.Entities;
using Dal.Abstractions.Models;
using Dal.Abstractions.Repositories;
using Dal.Context;
using Microsoft.EntityFrameworkCore;

namespace Dal.Repositories;

internal sealed class OrderRepository(OrderDbContext dbContext)
    : IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        await dbContext.Orders.AddAsync(order, cancellationToken);
    }

    public Task<Order?> GetByIdAsync(GetOrderByIdRepositoryModel repositoryModel, CancellationToken cancellationToken)
    {
        return dbContext.Orders
            .AsNoTracking()
            .Include(x => x.ArchiveFile)
            .FirstOrDefaultAsync(x => x.Id == repositoryModel.Id, cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> ListByUserAsync(
        ListOrdersByUserRepositoryModel repositoryModel,
        CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .AsNoTracking()
            .Include(x => x.ArchiveFile)
            .Where(x => x.UserId == repositoryModel.UserId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken)
    {
        dbContext.Orders.Update(order);
        return Task.CompletedTask;
    }
}