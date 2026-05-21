using Dal.Abstractions.Entities;
using FluentAssertions;
using Moq;
using VideoArchiveService.UnitTests.Fixtures;
using Xunit;

namespace VideoArchiveService.UnitTests.Operations;

public sealed class ListCamerasOperationTests
{
    private readonly VideoArchiveServiceFixture _fixture = new();

    [Fact]
    public async Task Should_Return_Empty_When_No_Cameras()
    {
        var sut = _fixture.CreateListSut();

        _fixture.CameraRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Camera>());

        var result = await sut.ExecuteAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();

        _fixture.VideoSegmentRepositoryMock.Verify(x =>
            x.ListByCameraIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Should_Map_Cameras_With_Their_Segments()
    {
        var sut = _fixture.CreateListSut();

        var cameraId = Guid.NewGuid();
        var camera = new Camera { Id = cameraId, Name = "Cam-1", IsActive = true };

        _fixture.CameraRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { camera });

        var segment = new VideoSegment
        {
            Id = Guid.NewGuid(),
            CameraId = cameraId,
            FromUtc = DateTimeOffset.UtcNow.AddHours(-2),
            ToUtc = DateTimeOffset.UtcNow
        };

        _fixture.VideoSegmentRepositoryMock
            .Setup(x => x.ListByCameraIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { segment });

        var result = await sut.ExecuteAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle();

        var model = result.Value[0];
        model.Id.Should().Be(cameraId);
        model.Name.Should().Be("Cam-1");
        model.Segments.Should().ContainSingle();
        model.Segments[0].FromUtc.Should().Be(segment.FromUtc);
    }
}
