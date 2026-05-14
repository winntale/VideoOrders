namespace Core.Abstractions.Operations;

public interface IReleaseResourcesOperation
{
    Task<int> ExecuteAsync(Guid orderId, CancellationToken cancellationToken);
}
