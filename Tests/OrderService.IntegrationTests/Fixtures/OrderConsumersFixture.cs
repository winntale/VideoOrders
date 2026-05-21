using Dal.Abstractions.Common;
using Dal.Abstractions.Entities;
using Dal.Abstractions.Models;
using Dal.Abstractions.Repositories;
using Dal.Common;
using Dal.Context;
using Dal.Repositories;
using Gateway.Consumers;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace OrderService.IntegrationTests.Fixtures;

public sealed class OrderConsumersFixture : IAsyncLifetime
{
    private SqliteConnection _connection = null!;

    public ServiceProvider ServiceProvider { get; private set; } = null!;
    public ITestHarness Harness { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var services = new ServiceCollection();

        services.AddDbContext<OrderDbContext>(options => options.UseSqlite(_connection));
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddMassTransitTestHarness(x =>
        {
            x.AddConsumer<OrderResourceReservedEventConsumer>();
            x.AddConsumer<ResourceReservationFailedEventConsumer>();
            x.AddConsumer<ProcessingStartedEventConsumer>();
        });

        ServiceProvider = services.BuildServiceProvider(true);

        await using (var scope = ServiceProvider.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            await dbContext.Database.EnsureCreatedAsync();
        }

        Harness = ServiceProvider.GetRequiredService<ITestHarness>();
        await Harness.Start();
    }

    public async Task<Order> SeedOrderAsync(OrderStatusSeed status = OrderStatusSeed.Created)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CameraId = Guid.NewGuid(),
            FromUtc = DateTimeOffset.UtcNow.AddHours(-1),
            ToUtc = DateTimeOffset.UtcNow,
            Status = (Dal.Abstractions.Enums.OrderStatus)status,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        await using var scope = ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync();

        return order;
    }

    public async Task<Order?> GetOrderAsync(Guid orderId)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        return await repository.GetByIdAsync(new GetOrderByIdRepositoryModel { Id = orderId }, CancellationToken.None);
    }

    public async Task DisposeAsync()
    {
        await Harness.Stop();
        await ServiceProvider.DisposeAsync();
        await _connection.DisposeAsync();
    }
}

public enum OrderStatusSeed
{
    Created = 0,
    ResourceReserved = 1,
    ProcessingStarted = 3
}
