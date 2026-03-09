using Dal.Abstractions.Entities;
using Dal.Abstractions.Models;

namespace Dal.Abstractions.Repositories;

public interface IOrderRepository
{
    Task AddAsync(
        Order order,
        CancellationToken cancellationToken);

    Task<Order?> GetByIdAsync(
        GetOrderByIdRepositoryModel repositoryModel,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Order order, 
        CancellationToken cancellationToken);
}