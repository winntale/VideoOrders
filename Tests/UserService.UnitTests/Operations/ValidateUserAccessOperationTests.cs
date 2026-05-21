using Core.Abstractions;
using Core.Abstractions.OperationModels;
using Dal.Abstractions.Entities;
using Dal.Abstractions.Enums;
using FluentAssertions;
using Moq;
using UserService.UnitTests.Fixtures;
using Xunit;

namespace UserService.UnitTests.Operations;

public sealed class ValidateUserAccessOperationTests
{
    private readonly UserServiceFixture _fixture = new();

    private static ValidateUserAccessOperationModel Model(Guid userId, Guid cameraId) =>
        new() { UserId = userId, CameraId = cameraId };

    [Fact]
    public async Task Should_Return_NotFound_When_User_Missing()
    {
        var sut = _fixture.CreateValidateAccessSut();

        _fixture.UserRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await sut.ExecuteAsync(Model(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Should_Deny_When_User_Not_Active()
    {
        var sut = _fixture.CreateValidateAccessSut();
        var userId = Guid.NewGuid();

        _fixture.UserRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, Login = "john", PasswordHash = "x", Status = UserStatus.Blocked });

        var result = await sut.ExecuteAsync(Model(userId, Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsAllowed.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Allow_When_User_Has_Access()
    {
        var sut = _fixture.CreateValidateAccessSut();
        var userId = Guid.NewGuid();
        var cameraId = Guid.NewGuid();

        _fixture.UserRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, Login = "john", PasswordHash = "x", Status = UserStatus.Active });
        _fixture.UserCameraAccessRepositoryMock
            .Setup(x => x.HasAccessAsync(userId, cameraId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await sut.ExecuteAsync(Model(userId, cameraId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsAllowed.Should().BeTrue();
        result.Value.DenyReason.Should().BeNull();
    }

    [Fact]
    public async Task Should_Deny_When_User_Has_No_Access()
    {
        var sut = _fixture.CreateValidateAccessSut();
        var userId = Guid.NewGuid();
        var cameraId = Guid.NewGuid();

        _fixture.UserRepositoryMock
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, Login = "john", PasswordHash = "x", Status = UserStatus.Active });
        _fixture.UserCameraAccessRepositoryMock
            .Setup(x => x.HasAccessAsync(userId, cameraId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await sut.ExecuteAsync(Model(userId, cameraId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsAllowed.Should().BeFalse();
        result.Value.DenyReason.Should().NotBeNull();
    }
}
