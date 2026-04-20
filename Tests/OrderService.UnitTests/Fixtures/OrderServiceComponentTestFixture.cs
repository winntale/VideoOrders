using AutoMapper;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Core.Operations;
using Dal.Abstractions.Common;
using Dal.Abstractions.Repositories;
using UserServiceClient.Abstractions.Clients;
using VideoArchiveClient.Abstractions.Clients;

namespace OrderService.UnitTests.Fixtures;

public sealed class OrderServiceFixture
{
    public Mock<IOrderRepository> OrderRepositoryMock { get; } = new();
    public Mock<IUnitOfWork> UnitOfWorkMock { get; } = new();
    public Mock<IMapper> MapperMock { get; } = new();
    public Mock<IUserServiceApiClient> UserServiceApiClientMock { get; } = new();
    public Mock<IVideoArchiveServiceApiClient> VideoArchiveServiceApiClientMock { get; } = new();
    public Mock<IPublishEndpoint> PublishEndpointMock { get; } = new();

    public IServiceProvider ServiceProvider { get; }

    public OrderServiceFixture()
    {
        var services = new ServiceCollection();

        services.AddScoped(_ => OrderRepositoryMock.Object);
        services.AddScoped(_ => UnitOfWorkMock.Object);
        services.AddScoped(_ => MapperMock.Object);
        services.AddScoped(_ => UserServiceApiClientMock.Object);
        services.AddScoped(_ => VideoArchiveServiceApiClientMock.Object);
        services.AddScoped(_ => PublishEndpointMock.Object);

        ServiceProvider = services.BuildServiceProvider();
    }

    internal CreateOrderOperation CreateSut()
    {
        return new CreateOrderOperation(
            OrderRepositoryMock.Object,
            UnitOfWorkMock.Object,
            MapperMock.Object,
            PublishEndpointMock.Object,
            VideoArchiveServiceApiClientMock.Object,
            UserServiceApiClientMock.Object);
    }
}