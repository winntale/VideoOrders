using AutoMapper;
using Core.Operations;
using Dal.Abstractions.Repositories;
using Moq;

namespace VideoArchiveService.UnitTests.Fixtures;

public sealed class VideoArchiveServiceFixture
{
    public Mock<ICameraRepository> CameraRepositoryMock { get; } = new();
    public Mock<IVideoSegmentRepository> VideoSegmentRepositoryMock { get; } = new();
    public Mock<IMapper> MapperMock { get; } = new();

    internal ListCamerasOperation CreateListSut() =>
        new(CameraRepositoryMock.Object, VideoSegmentRepositoryMock.Object);

    internal ValidateArchiveAvailabilityOperation CreateValidateSut() =>
        new(CameraRepositoryMock.Object, VideoSegmentRepositoryMock.Object, MapperMock.Object);
}
