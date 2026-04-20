using AutoMapper;
using Core.Abstractions;
using Core.Abstractions.Enums;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Dal.Abstractions.Repositories;
using Dal.Abstractions.Common;
using Dal.Abstractions.Entities;
using Events.Abstractions.Models;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using UserServiceClient.Abstractions.Models;
using UserServiceClient.Abstractions.Clients;
using UserServiceClient.Abstractions.Enums;
using VideoArchiveClient.Abstractions.Clients;
using VideoArchiveClient.Abstractions.Enums;
using VideoArchiveClient.Abstractions.Models;

namespace Core.Operations;

internal sealed class CreateOrderOperation(
    IOrderRepository repository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPublishEndpoint publishEndpoint,
    [FromKeyedServices(nameof(VideoArchiveServiceEnum.VideoArchiveServiceApiClientKey))]
    IVideoArchiveServiceApiClient videoArchiveServiceApiClient,
    [FromKeyedServices(nameof(UserServiceEnum.UserServiceApiClientKey))]
    IUserServiceApiClient userServiceApiClient)
    : ICreateOrderOperation
{
    public async Task<Result<OrderDetailsOperationModel>> ExecuteAsync(
        CreateOrderOperationModel operationModel,
        CancellationToken cancellationToken)
    {

        var videoArchiveServiceClientModel = mapper.Map<ValidateArchiveAvailabilityClientModel>(operationModel);
        var videoArchiveServiceClientResult =
            await videoArchiveServiceApiClient.ValidateArchiveAvailabilityAsync(
                videoArchiveServiceClientModel,
                cancellationToken);

        if (videoArchiveServiceClientResult.IsFailure)
        {
            return videoArchiveServiceClientResult.Error.Type switch
            {
                VideoArchiveClient.Abstractions.ErrorType.NotFound =>
                    Error.NotFound(videoArchiveServiceClientResult.Error.Message),
                _ => Error.Failure(videoArchiveServiceClientResult.Error.Message)
            };
        }
        
        if (videoArchiveServiceClientResult.Value is null)
        {
            return Error.Failure(videoArchiveServiceClientResult.Error.Message);
        }

        var userServiceClientModel = mapper.Map<ValidateAccessClientModel>(operationModel);
        var userServiceCallResult = await userServiceApiClient.ValidateUserAccessAsync(
            userServiceClientModel,
            cancellationToken);

        if (userServiceCallResult.IsFailure)
        {
            return userServiceCallResult.Error.Type switch
            {
                UserServiceClient.Abstractions.ErrorType.NotFound =>
                    Error.NotFound(userServiceCallResult.Error.Message),
                _ => Error.Failure(userServiceCallResult.Error.Message)
            };
        }

        if (userServiceCallResult.Value is null)
        {
            return Error.Failure(userServiceCallResult.Error.Message);
        }

        if (!userServiceCallResult.Value.IsAllowed)
        {
            return Error.Forbidden(
                userServiceCallResult.Value.DenyReason ??
                userServiceCallResult.Error.Message);
        }
        
        if (!videoArchiveServiceClientResult.Value.IsAvailable)
        {
            return Error.Forbidden(
                videoArchiveServiceClientResult.Value.DenyReason ??
                videoArchiveServiceClientResult.Error.Message);
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

        var orderCreatedEvent = mapper.Map<OrderCreatedEvent>(order);
        await publishEndpoint.Publish(orderCreatedEvent, cancellationToken);
        
        var resultModel = mapper.Map<OrderDetailsOperationModel>(order);
        return resultModel;
    }
}