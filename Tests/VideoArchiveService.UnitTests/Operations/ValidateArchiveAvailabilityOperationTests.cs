using Core.Abstractions;
using Core.Abstractions.OperationModels;
using Dal.Abstractions.Entities;
using Dal.Abstractions.Models;
using FluentAssertions;
using Moq;
using VideoArchiveService.UnitTests.Fixtures;
using Xunit;

namespace VideoArchiveService.UnitTests.Operations;

public sealed class ValidateArchiveAvailabilityOperationTests
{
    private readonly VideoArchiveServiceFixture _fixture = new();

    private ValidateArchiveAvailabilityOperationModel Arrange(Camera? camera, VideoSegment? coveringSegment)
    {
        var cameraId = camera?.Id ?? Guid.NewGuid();
        var model = new ValidateArchiveAvailabilityOperationModel
        {
            CameraId = cameraId,
            FromUtc = DateTimeOffset.UtcNow.AddHours(-1),
            ToUtc = DateTimeOffset.UtcNow
        };

        _fixture.MapperMock
            .Setup(x => x.Map<CameraRepositoryModel>(model))
            .Returns(new CameraRepositoryModel { CameraId = cameraId });
        _fixture.MapperMock
            .Setup(x => x.Map<VideoSegmentRepositoryModel>(model))
            .Returns(new VideoSegmentRepositoryModel
            {
                CameraId = cameraId,
                FromUtc = model.FromUtc,
                ToUtc = model.ToUtc
            });

        _fixture.CameraRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<CameraRepositoryModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(camera);
        _fixture.VideoSegmentRepositoryMock
            .Setup(x => x.GetCoveringSegmentAsync(It.IsAny<VideoSegmentRepositoryModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(coveringSegment);

        return model;
    }

    [Fact]
    public async Task Should_Return_NotFound_When_Camera_Missing()
    {
        var sut = _fixture.CreateValidateSut();
        var model = Arrange(camera: null, coveringSegment: null);

        var result = await sut.ExecuteAsync(model, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Should_Deny_When_Camera_Inactive()
    {
        var sut = _fixture.CreateValidateSut();
        var camera = new Camera { Id = Guid.NewGuid(), Name = "Cam", IsActive = false };
        var model = Arrange(camera, coveringSegment: null);

        var result = await sut.ExecuteAsync(model, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Be_Available_When_Covering_Segment_Exists()
    {
        var sut = _fixture.CreateValidateSut();
        var camera = new Camera { Id = Guid.NewGuid(), Name = "Cam", IsActive = true };
        var segment = new VideoSegment
        {
            Id = Guid.NewGuid(),
            CameraId = camera.Id,
            FromUtc = DateTimeOffset.UtcNow.AddHours(-3),
            ToUtc = DateTimeOffset.UtcNow
        };
        var model = Arrange(camera, segment);

        var result = await sut.ExecuteAsync(model, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsAvailable.Should().BeTrue();
        result.Value.SegmentStartUtc.Should().Be(segment.FromUtc);
    }

    [Fact]
    public async Task Should_Be_Unavailable_When_No_Covering_Segment()
    {
        var sut = _fixture.CreateValidateSut();
        var camera = new Camera { Id = Guid.NewGuid(), Name = "Cam", IsActive = true };
        var model = Arrange(camera, coveringSegment: null);

        var result = await sut.ExecuteAsync(model, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsAvailable.Should().BeFalse();
        result.Value.DenyReason.Should().NotBeNull();
    }
}
