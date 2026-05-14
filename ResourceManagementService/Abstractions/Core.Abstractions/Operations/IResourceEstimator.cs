using Core.Abstractions.OperationModels;

namespace Core.Abstractions.Operations;

public interface IResourceEstimator
{
    ResourceEstimateBundle Estimate(TimeSpan archiveDuration);
}
