using Dal.Abstractions.Common;
using Dal.Abstractions.Repositories;
using Gateway.Consumers;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace OrderService.UnitTests.Fixtures;

public class MassTransitOrderServiceFixture : IAsyncLifetime
{
    public ServiceProvider ServiceProvider { get; private set; } = null!;
    public ITestHarness Harness { get; private set; } = null!;

    public Mock<IOrderRepository> OrderRepositoryMock { get; } = new();
    public Mock<IUnitOfWork> UnitOfWorkMock { get; } = new();
    
    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();

        services.AddScoped(_ => OrderRepositoryMock.Object);
        services.AddScoped(_ => UnitOfWorkMock.Object);

        services.AddMassTransitTestHarness(x =>
        {
            x.AddConsumer<OrderResourceReservedEventConsumer>();
            x.AddConsumer<ResourceReservationFailedEventConsumer>();
        });

        ServiceProvider = services.BuildServiceProvider(true);
        Harness = ServiceProvider.GetRequiredService<ITestHarness>();

        await Harness.Start();
    }

    public async Task DisposeAsync()
    {
        await Harness.Stop();
        await ServiceProvider.DisposeAsync();
    }
}