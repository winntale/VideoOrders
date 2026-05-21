using Dal.Abstractions.Enums;
using Events.Abstractions.Models;
using FluentAssertions;
using MassTransit.Testing;
using OrderService.IntegrationTests.Fixtures;
using Xunit;

namespace OrderService.IntegrationTests;

public sealed class OrderLifecycleConsumerTests : IClassFixture<OrderConsumersFixture>
{
    private readonly OrderConsumersFixture _fixture;

    public OrderLifecycleConsumerTests(OrderConsumersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ResourceReserved_Moves_Created_Order_To_ResourceReserved()
    {
        var order = await _fixture.SeedOrderAsync();

        await _fixture.Harness.Bus.Publish(new ResourceReservedEvent
        {
            OrderId = order.Id,
            CameraId = order.CameraId,
            FromUtc = order.FromUtc,
            ToUtc = order.ToUtc,
            ReservedAtUtc = DateTimeOffset.UtcNow
        });

        (await _fixture.Harness.Consumed.Any<ResourceReservedEvent>(
            m => m.Context.Message.OrderId == order.Id)).Should().BeTrue();

        var updated = await _fixture.GetOrderAsync(order.Id);
        updated.Should().NotBeNull();
        updated!.Status.Should().Be(OrderStatus.ResourceReserved);
    }

    [Fact]
    public async Task ResourceReservationFailed_Moves_Created_Order_To_Failed_With_Reason()
    {
        var order = await _fixture.SeedOrderAsync();

        await _fixture.Harness.Bus.Publish(new ResourceReservationFailedEvent
        {
            OrderId = order.Id,
            Reason = "Недостаточно ресурса Disk.",
            FailedAtUtc = DateTimeOffset.UtcNow
        });

        (await _fixture.Harness.Consumed.Any<ResourceReservationFailedEvent>(
            m => m.Context.Message.OrderId == order.Id)).Should().BeTrue();

        var updated = await _fixture.GetOrderAsync(order.Id);
        updated.Should().NotBeNull();
        updated!.Status.Should().Be(OrderStatus.ResourceReservationFailed);
        updated.FailureReason.Should().Be("Недостаточно ресурса Disk.");
    }

    [Fact]
    public async Task ProcessingStarted_Moves_ResourceReserved_Order_To_ProcessingStarted()
    {
        var order = await _fixture.SeedOrderAsync(OrderStatusSeed.ResourceReserved);

        await _fixture.Harness.Bus.Publish(new ProcessingStartedEvent
        {
            OrderId = order.Id,
            StartedAtUtc = DateTimeOffset.UtcNow
        });

        (await _fixture.Harness.Consumed.Any<ProcessingStartedEvent>(
            m => m.Context.Message.OrderId == order.Id)).Should().BeTrue();

        var updated = await _fixture.GetOrderAsync(order.Id);
        updated.Should().NotBeNull();
        updated!.Status.Should().Be(OrderStatus.ProcessingStarted);
    }

    [Fact]
    public async Task ResourceReserved_For_Unknown_Order_Is_Consumed_Without_Error()
    {
        var unknownOrderId = Guid.NewGuid();

        await _fixture.Harness.Bus.Publish(new ResourceReservedEvent
        {
            OrderId = unknownOrderId,
            CameraId = Guid.NewGuid(),
            FromUtc = DateTimeOffset.UtcNow.AddHours(-1),
            ToUtc = DateTimeOffset.UtcNow,
            ReservedAtUtc = DateTimeOffset.UtcNow
        });

        (await _fixture.Harness.Consumed.Any<ResourceReservedEvent>(
            m => m.Context.Message.OrderId == unknownOrderId)).Should().BeTrue();

        var order = await _fixture.GetOrderAsync(unknownOrderId);
        order.Should().BeNull();
    }
}
