using Dal.Abstractions.Entities;
using Dal.Abstractions.Enums;
using FluentAssertions;
using Moq;
using ResourceManagementService.UnitTests.Fixtures;
using Xunit;

namespace ResourceManagementService.UnitTests.Operations;

public sealed class ReserveResourcesOperationTests
{
    private readonly ResourceManagementFixture _fixture = new();

    private static Resource Res(ResourceType type, long capacity, long reserved, string unit) => new()
    {
        Id = Guid.NewGuid(),
        Type = type,
        TotalCapacity = capacity,
        ReservedAmount = reserved,
        Unit = unit
    };

    [Fact]
    public async Task Should_Reserve_When_Capacity_Sufficient()
    {
        var sut = _fixture.CreateReserveSut();

        var cpu = Res(ResourceType.Cpu, 16, 0, "cores");
        var ram = Res(ResourceType.Ram, 8192, 0, "MB");
        var disk = Res(ResourceType.Disk, 100_000, 0, "MB");

        _fixture.ResourceRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { cpu, ram, disk });

        var from = DateTimeOffset.UtcNow.AddHours(-1);
        var to = DateTimeOffset.UtcNow;

        var outcome = await sut.ExecuteAsync(Guid.NewGuid(), from, to, CancellationToken.None);

        outcome.Success.Should().BeTrue();
        cpu.ReservedAmount.Should().BeGreaterThan(0);
        disk.ReservedAmount.Should().BeGreaterThan(0);

        _fixture.ReservationRepositoryMock.Verify(x =>
            x.AddRangeAsync(It.IsAny<IEnumerable<Reservation>>(), It.IsAny<CancellationToken>()), Times.Once);
        _fixture.UnitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _fixture.TransactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Fail_When_Resource_Not_Registered()
    {
        var sut = _fixture.CreateReserveSut();

        _fixture.ResourceRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                Res(ResourceType.Cpu, 16, 0, "cores"),
                Res(ResourceType.Ram, 8192, 0, "MB")
            });

        var outcome = await sut.ExecuteAsync(
            Guid.NewGuid(), DateTimeOffset.UtcNow.AddHours(-1), DateTimeOffset.UtcNow, CancellationToken.None);

        outcome.Success.Should().BeFalse();
        outcome.FailureReason.Should().Contain(ResourceType.Disk.ToString());

        _fixture.ReservationRepositoryMock.Verify(x =>
            x.AddRangeAsync(It.IsAny<IEnumerable<Reservation>>(), It.IsAny<CancellationToken>()), Times.Never);
        _fixture.TransactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Fail_When_Capacity_Insufficient()
    {
        var sut = _fixture.CreateReserveSut();

        _fixture.ResourceRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                Res(ResourceType.Cpu, 16, 0, "cores"),
                Res(ResourceType.Ram, 8192, 0, "MB"),
                Res(ResourceType.Disk, 10, 0, "MB")
            });

        var outcome = await sut.ExecuteAsync(
            Guid.NewGuid(), DateTimeOffset.UtcNow.AddHours(-1), DateTimeOffset.UtcNow, CancellationToken.None);

        outcome.Success.Should().BeFalse();
        outcome.FailureReason.Should().NotBeNullOrEmpty();

        _fixture.ReservationRepositoryMock.Verify(x =>
            x.AddRangeAsync(It.IsAny<IEnumerable<Reservation>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
