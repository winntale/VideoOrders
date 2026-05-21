using Core.Abstractions;
using Core.Abstractions.OperationModels;
using Dal.Abstractions.Entities;
using FluentAssertions;
using Moq;
using UserService.UnitTests.Fixtures;
using Xunit;

namespace UserService.UnitTests.Operations;

public sealed class RegisterUserOperationTests
{
    private readonly UserServiceFixture _fixture = new();

    [Fact]
    public async Task Should_Register_User_When_Login_Is_Free()
    {
        var sut = _fixture.CreateRegisterSut();

        _fixture.UserRepositoryMock
            .Setup(x => x.GetByLoginAsync("john", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var model = new RegisterUserOperationModel { Login = "  john  ", Password = "secret123" };

        var result = await sut.ExecuteAsync(model, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Login.Should().Be("john");

        _fixture.UserRepositoryMock.Verify(x =>
            x.AddAsync(It.Is<User>(u => u.Login == "john" && u.PasswordHash != "secret123"),
                It.IsAny<CancellationToken>()), Times.Once);
        _fixture.UnitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Fail_When_Login_Too_Short()
    {
        var sut = _fixture.CreateRegisterSut();

        var model = new RegisterUserOperationModel { Login = "ab", Password = "secret123" };

        var result = await sut.ExecuteAsync(model, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
        _fixture.UserRepositoryMock.Verify(x =>
            x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Fail_When_Password_Too_Short()
    {
        var sut = _fixture.CreateRegisterSut();

        var model = new RegisterUserOperationModel { Login = "john", Password = "123" };

        var result = await sut.ExecuteAsync(model, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task Should_Fail_With_Conflict_When_Login_Taken()
    {
        var sut = _fixture.CreateRegisterSut();

        _fixture.UserRepositoryMock
            .Setup(x => x.GetByLoginAsync("john", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = Guid.NewGuid(), Login = "john", PasswordHash = "x" });

        var model = new RegisterUserOperationModel { Login = "john", Password = "secret123" };

        var result = await sut.ExecuteAsync(model, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Conflict);
        _fixture.UserRepositoryMock.Verify(x =>
            x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
