using Dal.Abstractions.Entities;
using Dal.Abstractions.Enums;
using FluentAssertions;
using Dal.Repositories;
using OrderService.UnitTests.Fixtures;
using Xunit;

namespace OrderService.UnitTests.Repositories;

public sealed class OrderRepositoryTests : IClassFixture<OrderRepositoryFixture>
{
    private readonly OrderRepositoryFixture _fixture;

    public OrderRepositoryTests(OrderRepositoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Should_Add_Order()
    {
        var repository = new OrderRepository(_fixture.DbContext);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CameraId = Guid.NewGuid(),
            FromUtc = DateTimeOffset.UtcNow.AddHours(-1),
            ToUtc = DateTimeOffset.UtcNow,
            Status = OrderStatus.Created,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        await repository.AddAsync(order, CancellationToken.None);
        await _fixture.DbContext.SaveChangesAsync();

        var saved = await _fixture.DbContext.Orders.FindAsync(order.Id);

        saved.Should().NotBeNull();
        saved!.Id.Should().Be(order.Id);
    }
}