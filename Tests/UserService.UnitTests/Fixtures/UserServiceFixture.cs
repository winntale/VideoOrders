using AutoMapper;
using Core.Operations;
using Dal.Abstractions.Common;
using Dal.Abstractions.Repositories;
using Moq;

namespace UserService.UnitTests.Fixtures;

public sealed class UserServiceFixture
{
    public Mock<IUserRepository> UserRepositoryMock { get; } = new();
    public Mock<IUserCameraAccessRepository> UserCameraAccessRepositoryMock { get; } = new();
    public Mock<IUnitOfWork> UnitOfWorkMock { get; } = new();
    public Mock<IMapper> MapperMock { get; } = new();

    internal RegisterUserOperation CreateRegisterSut() =>
        new(UserRepositoryMock.Object, UnitOfWorkMock.Object);

    internal LoginUserOperation CreateLoginSut() =>
        new(UserRepositoryMock.Object);

    internal ValidateUserAccessOperation CreateValidateAccessSut() =>
        new(UserRepositoryMock.Object, UserCameraAccessRepositoryMock.Object, MapperMock.Object);
}
