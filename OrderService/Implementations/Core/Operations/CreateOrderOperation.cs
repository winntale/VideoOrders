using AutoMapper;
using Core.Abstractions;
using Core.Abstractions.Enums;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Dal.Abstractions.Repositories;
using Dal.Abstractions.Common;
using Dal.Abstractions.Entities;
using UserService.Abstractions.Models;
using UserService.Abstractions.Clients;

namespace Core.Operations;

internal sealed class CreateOrderOperation(
    IOrderRepository repository,
    IUnitOfWork unitOfWork,
    IUserServiceApiClient userServiceApiClient,
    IMapper mapper)
    : ICreateOrderOperation
{
    public async Task<Result<OrderDetailsOperationModel>> ExecuteAsync(
        CreateOrderOperationModel operationModel,
        CancellationToken cancellationToken)
    {

        var clientModel = mapper.Map<ValidateAccessClientModel>(operationModel);
        var callResult = await userServiceApiClient.ValidateUserAccessAsync(
            clientModel,
            cancellationToken);

        if (callResult is null)
        {
            return Error.Failure("Unable to validate user access due to UserService error.");
        }
        
        if (callResult.UserNotFound)
        {
            return Error.NotFound(
                callResult.DenyReason ??
                $"User with id '{operationModel.UserId}' was not found.");
        }

        if (!callResult.IsAllowed)
        {
            return Error.Forbidden(
                callResult.DenyReason ?? "User has no access to this camera.");
        }
        
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