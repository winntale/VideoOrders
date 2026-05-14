using Core.Abstractions.Operations;
using Dal.Abstractions.Common;
using Dal.Abstractions.Enums;
using Dal.Abstractions.Repositories;

namespace Core.Operations;

internal sealed class ReleaseResourcesOperation(
    IReservationRepository reservations,
    IResourceRepository resources,
    IUnitOfWork unitOfWork)
    : IReleaseResourcesOperation
{
    public async Task<int> ExecuteAsync(Guid orderId, CancellationToken cancellationToken)
    {
        await using var tx = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var active = await reservations.GetActiveByOrderIdAsync(orderId, cancellationToken);
        if (active.Count == 0)
        {
            return 0;
        }

        var releasedAt = DateTimeOffset.UtcNow;
        var pool = (await resources.GetAllAsync(cancellationToken)).ToDictionary(r => r.Type);

        foreach (var reservation in active)
        {
            reservation.Status = ReservationStatus.Released;
            reservation.ReleasedAtUtc = releasedAt;

            if (pool.TryGetValue(reservation.ResourceType, out var res))
            {
                res.ReservedAmount = Math.Max(0, res.ReservedAmount - reservation.Amount);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        return active.Count;
    }
}
