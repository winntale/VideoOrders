using Dal.Abstractions.Entities;
using Dal.Abstractions.Models;
using Dal.Abstractions.Repositories;
using Dal.Context;
using Microsoft.EntityFrameworkCore;

namespace Dal.Repositories;

public class OrderRepository(OrderDbContext dbContext)
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
            .FirstOrDefaultAsync(x => x.Id == repositoryModel.Id, cancellationToken);
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken)
    {
        dbContext.Orders.Update(order);
        return Task.CompletedTask;
    }
}