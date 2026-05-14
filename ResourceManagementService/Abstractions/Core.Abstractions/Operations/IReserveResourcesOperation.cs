using Core.Abstractions.OperationModels;

namespace Core.Abstractions.Operations;

public interface IReserveResourcesOperation
{
    Task<ReservationOutcome> ExecuteAsync(
        Guid orderId,
        DateTimeOffset fromUtc,
        DateTimeOffset toUtc,
        CancellationToken cancellationToken);
}
