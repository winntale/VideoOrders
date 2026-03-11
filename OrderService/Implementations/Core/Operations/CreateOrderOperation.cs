using AutoMapper;
using Core.Abstractions;
using Core.Abstractions.Enums;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Dal.Abstractions.Repositories;
using Dal.Abstractions.Common;
using Dal.Abstractions.Entities;

namespace Core.Operations;

internal sealed class CreateOrderOperation(
    IOrderRepository repository,
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : ICreateOrderOperation
{
    public async Task<Result<OrderDetailsOperationModel>> ExecuteAsync(
        CreateOrderOperationModel operationModel,
        CancellationToken cancellationToken)
    {
        var order = mapper.Map<Order>(operationModel, opts =>
        {
            opts.Items["Id"] = Guid.NewGuid();
            opts.Items["Status"] = OrderStatus.Created;
            opts.Items["FailureReason"] = null;
            opts.Items["CreatedAtUtc"] = DateTimeOffset.UtcNow;
            opts.Items["UpdatedAtUtc"] = DateTimeOffset.UtcNow;
        });
        
        await repository.AddAsync(order, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var resultModel = mapper.Map<OrderDetailsOperationModel>(order);
        return resultModel;
    }
}