using Dal.Abstractions.Entities;
using Dal.Abstractions.Enums;
using FluentAssertions;
using Moq;
using ResourceManagementService.UnitTests.Fixtures;
using Xunit;

namespace ResourceManagementService.UnitTests.Operations;

public sealed class ReleaseResourcesOperationTests
{
    private readonly ResourceManagementFixture _fixture = new();

    [Fact]
    public async Task Should_Return_Zero_When_No_Active_Reservations()
    {
        var sut = _fixture.CreateReleaseSut();
        var orderId = Guid.NewGuid();

        _fixture.ReservationRepositoryMock
            .Setup(x => x.GetActiveByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Reservation>());

        var released = await sut.ExecuteAsync(orderId, CancellationToken.None);

        released.Should().Be(0);
        _fixture.UnitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _fixture.TransactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Release_Reservations_And_Free_Capacity()
    {
        var sut = _fixture.CreateReleaseSut();
        var orderId = Guid.NewGuid();

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ResourceType = ResourceType.Disk,
            Amount = 600,
            Status = ReservationStatus.Active
        };

        var disk = new Resource
        {
            Id = Guid.NewGuid(),
            Type = ResourceType.Disk,
            TotalCapacity = 100_000,
            ReservedAmount = 600,
            Unit = "MB"
        };

        _fixture.ReservationRepositoryMock
            .Setup(x => x.GetActiveByOrderIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { reservation });
        _fixture.ResourceRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { disk });

        var released = await sut.ExecuteAsync(orderId, CancellationToken.None);

        released.Should().Be(1);
        reservation.Status.Should().Be(ReservationStatus.Released);
        reservation.ReleasedAtUtc.Should().NotBeNull();
        disk.ReservedAmount.Should().Be(0);

        _fixture.UnitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _fixture.TransactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
