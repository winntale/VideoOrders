using AutoMapper;
using Core.Abstractions;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Dal.Abstractions.Common;
using Dal.Abstractions.Enums;
using Dal.Abstractions.Models;
using Dal.Abstractions.Repositories;

namespace Core.Operations;

internal sealed class ChangeOrderStatusOperation(
    IOrderRepository repository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IChangeOrderStatusOperation
{
    public async Task<Result<OrderDetailsOperationModel>> ExecuteAsync(
        ChangeOrderStatusOperationModel operationModel,
        CancellationToken cancellationToken)
    {
        var repositoryModel = mapper.Map<GetOrderByIdRepositoryModel>(operationModel);
        
        var order = await repository.GetByIdAsync(repositoryModel, cancellationToken);
        if (order is null)
        {
            return Error.NotFound($"Order with id '{repositoryModel.Id}' was not found.");
        }

        order.Status = (OrderStatus)operationModel.Status;
        order.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await repository.UpdateAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var model = mapper.Map<OrderDetailsOperationModel>(order);
        return model;
    }
}