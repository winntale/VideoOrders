using Core.Abstractions.OperationModels;
using Core.Operations;
using Dal.Abstractions.Enums;
using FluentAssertions;
using Xunit;

namespace ResourceManagementService.UnitTests;

public sealed class ResourceEstimatorTests
{
    private readonly ResourceEstimator _sut = new();

    [Fact]
    public void Estimate_ReturnsAllThreeResourceTypes()
    {
        var bundle = _sut.Estimate(TimeSpan.FromHours(2));

        bundle.Resources.Select(e => e.Type).Should().BeEquivalentTo(new[]
        {
            ResourceType.Cpu, ResourceType.Ram, ResourceType.Disk,
        });
    }

    [Fact]
    public void Estimate_DiskScalesLinearlyWithArchiveDuration()
    {
        var oneHour = Disk(_sut.Estimate(TimeSpan.FromHours(1)));
        var twoHours = Disk(_sut.Estimate(TimeSpan.FromHours(2)));

        // 8 МБ/мин * 60 мин * 1.25 overhead = 600 МБ
        oneHour.Should().BeInRange(500, 700);
        twoHours.Should().BeInRange(1000, 1400);
    }

    [Fact]
    public void Estimate_CpuStaysConstantRegardlessOfDuration()
    {
        var shortRun = Cpu(_sut.Estimate(TimeSpan.FromMinutes(5)));
        var longRun = Cpu(_sut.Estimate(TimeSpan.FromHours(10)));

        shortRun.Should().Be(longRun);
    }

    [Fact]
    public void Estimate_ProcessingDurationIsSubstantiallyShorterThanArchive()
    {
        var bundle = _sut.Estimate(TimeSpan.FromHours(4));

        // 4h архива / 4× ≈ 1h обработки + overhead
        bundle.EstimatedProcessingDuration.Should().BeLessThan(TimeSpan.FromHours(2));
        bundle.EstimatedProcessingDuration.Should().BeGreaterThan(TimeSpan.FromMinutes(30));
    }

    [Fact]
    public void Estimate_ZeroDuration_StillReturnsMinimumAmountsAndDuration()
    {
        var bundle = _sut.Estimate(TimeSpan.Zero);

        bundle.Resources.Should().OnlyContain(e => e.Amount > 0);
        bundle.EstimatedProcessingDuration.Should().BeGreaterThanOrEqualTo(TimeSpan.FromSeconds(15));
    }

    private static long Disk(ResourceEstimateBundle b) =>
        b.Resources.First(x => x.Type == ResourceType.Disk).Amount;

    private static long Cpu(ResourceEstimateBundle b) =>
        b.Resources.First(x => x.Type == ResourceType.Cpu).Amount;
}
