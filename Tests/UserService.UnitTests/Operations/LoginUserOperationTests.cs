using Core.Abstractions;
using Core.Abstractions.OperationModels;
using Dal.Abstractions.Entities;
using Dal.Abstractions.Enums;
using FluentAssertions;
using Moq;
using UserService.UnitTests.Fixtures;
using Xunit;

namespace UserService.UnitTests.Operations;

public sealed class LoginUserOperationTests
{
    private readonly UserServiceFixture _fixture = new();

    private static User ActiveUser(string password) => new()
    {
        Id = Guid.NewGuid(),
        Login = "john",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
        Status = UserStatus.Active
    };

    [Fact]
    public async Task Should_Login_When_Credentials_Valid()
    {
        var sut = _fixture.CreateLoginSut();

        _fixture.UserRepositoryMock
            .Setup(x => x.GetByLoginAsync("john", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ActiveUser("secret123"));

        var result = await sut.ExecuteAsync(
            new LoginUserOperationModel { Login = "john", Password = "secret123" },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Login.Should().Be("john");
    }

    [Fact]
    public async Task Should_Fail_When_User_Not_Found()
    {
        var sut = _fixture.CreateLoginSut();

        _fixture.UserRepositoryMock
            .Setup(x => x.GetByLoginAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await sut.ExecuteAsync(
            new LoginUserOperationModel { Login = "john", Password = "secret123" },
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task Should_Fail_When_Password_Wrong()
    {
        var sut = _fixture.CreateLoginSut();

        _fixture.UserRepositoryMock
            .Setup(x => x.GetByLoginAsync("john", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ActiveUser("secret123"));

        var result = await sut.ExecuteAsync(
            new LoginUserOperationModel { Login = "john", Password = "wrong-password" },
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task Should_Fail_When_User_Not_Active()
    {
        var sut = _fixture.CreateLoginSut();

        var user = ActiveUser("secret123");
        user.Status = UserStatus.Blocked;

        _fixture.UserRepositoryMock
            .Setup(x => x.GetByLoginAsync("john", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var result = await sut.ExecuteAsync(
            new LoginUserOperationModel { Login = "john", Password = "secret123" },
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Forbidden);
    }
}
