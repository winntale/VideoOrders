using AutoMapper;
using Core.Abstractions.OperationModels;
using Dal.Abstractions.Entities;
using FluentAssertions;
using MassTransit;
using Moq;
using Core.Abstractions.OperationModels;
using Dal.Abstractions.Enums;
using OrderService.UnitTests.Fixtures;
using UserServiceClient.Abstractions.Models;
using VideoArchiveClient.Abstractions;
using VideoArchiveClient.Abstractions.Models;
using Xunit;

namespace OrderService.UnitTests.Operations;

public sealed class CreateOrderOperationTests : IClassFixture<OrderServiceFixture>
{
    private readonly OrderServiceFixture _fixture;

    public CreateOrderOperationTests(OrderServiceFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Should_Create_Order_And_Publish_OrderCreated_When_CameraExists_UserHasAccess_And_ArchiveAvailable()
    {
        var sut = _fixture.CreateSut();

        var operationModel = new CreateOrderOperationModel
        {
            UserId = Guid.NewGuid(),
            CameraId = Guid.NewGuid(),
            FromUtc = DateTimeOffset.UtcNow.AddHours(-1),
            ToUtc = DateTimeOffset.UtcNow
        };

        var createdOrder = new Order
        {
            Id = Guid.NewGuid(),
            UserId = operationModel.UserId,
            CameraId = operationModel.CameraId,
            FromUtc = operationModel.FromUtc,
            ToUtc = operationModel.ToUtc,
            Status = OrderStatus.Created,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        _fixture.MapperMock
            .Setup(x => x.Map<Order>(
                It.IsAny<CreateOrderOperationModel>(),
                It.IsAny<Action<IMappingOperationOptions<object, Order>>>()))
            .Returns(createdOrder);

        _fixture.VideoArchiveServiceApiClientMock
            .Setup(x => x.ValidateArchiveAvailabilityAsync(It.IsAny<ValidateArchiveAvailabilityClientModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidateArchiveAvailabilityResultClientModel
            {
                IsAvailable = true,
                DenyReason = ""
            });

        _fixture.UserServiceApiClientMock
            .Setup(x => x.ValidateUserAccessAsync(It.IsAny<ValidateAccessClientModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidateAccessResultClientModel
            {
                IsAllowed = true,
                DenyReason = ""
            });

        var result = await sut.ExecuteAsync(operationModel, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        _fixture.OrderRepositoryMock.Verify(x =>
                x.AddAsync(
                    It.Is<Order>(o =>
                        o != null &&
                        o.UserId == operationModel.UserId &&
                        o.CameraId == operationModel.CameraId),
                    It.IsAny<CancellationToken>()),
            Times.Once);

        _fixture.UnitOfWorkMock.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _fixture.PublishEndpointMock.Verify(x =>
            x.Publish(It.IsAny<Events.Abstractions.Models.OrderCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}