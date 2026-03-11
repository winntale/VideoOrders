using AutoMapper;
using Core.Abstractions;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Dal.Abstractions.Common;
using Dal.Abstractions.Models;
using Dal.Abstractions.Repositories;

namespace Core.Operations;

internal sealed class GetOrderByIdOperation(
    IOrderRepository repository,
    IMapper mapper) : IGetOrderByIdOperation
{
    public async Task<Result<OrderDetailsOperationModel>> ExecuteAsync(
        GetOrderByIdOperationModel operationModel,
        CancellationToken cancellationToken)
    {
        var repositoryModel = mapper.Map<GetOrderByIdRepositoryModel>(operationModel);
        
        var order = await repository.GetByIdAsync(repositoryModel, cancellationToken);
        if (order is null)
        {
            return Error.NotFound($"Order with id '{repositoryModel.Id}' was not found.");
        }

        var model = mapper.Map<OrderDetailsOperationModel>(order);
        return model;
    }
}