using AutoMapper;
using Core.Abstractions;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Dal.Abstractions.Models;
using Dal.Abstractions.Repositories;

namespace Core.Operations;

internal sealed class ListOrdersByUserOperation(
    IOrderRepository repository,
    IMapper mapper) : IListOrdersByUserOperation
{
    public async Task<Result<IReadOnlyList<OrderDetailsOperationModel>>> ExecuteAsync(
        ListOrdersByUserOperationModel operationModel,
        CancellationToken cancellationToken)
    {
        var repositoryModel = mapper.Map<ListOrdersByUserRepositoryModel>(operationModel);

        var orders = await repository.ListByUserAsync(repositoryModel, cancellationToken);

        var models = mapper.Map<IReadOnlyList<OrderDetailsOperationModel>>(orders);
        return Result<IReadOnlyList<OrderDetailsOperationModel>>.Success(models);
    }
}
