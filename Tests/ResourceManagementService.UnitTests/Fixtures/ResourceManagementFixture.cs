using Core.Operations;
using Dal.Abstractions.Common;
using Dal.Abstractions.Repositories;
using Moq;

namespace ResourceManagementService.UnitTests.Fixtures;

public sealed class ResourceManagementFixture
{
    public Mock<IResourceRepository> ResourceRepositoryMock { get; } = new();
    public Mock<IReservationRepository> ReservationRepositoryMock { get; } = new();
    public Mock<IUnitOfWork> UnitOfWorkMock { get; } = new();
    public Mock<IDisposableTransaction> TransactionMock { get; } = new();

    public ResourceManagementFixture()
    {
        UnitOfWorkMock
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(TransactionMock.Object);
    }

    internal ReserveResourcesOperation CreateReserveSut() =>
        new(new ResourceEstimator(), ResourceRepositoryMock.Object, ReservationRepositoryMock.Object, UnitOfWorkMock.Object);

    internal ReleaseResourcesOperation CreateReleaseSut() =>
        new(ReservationRepositoryMock.Object, ResourceRepositoryMock.Object, UnitOfWorkMock.Object);
}
