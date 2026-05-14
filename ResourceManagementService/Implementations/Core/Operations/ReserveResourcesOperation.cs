using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Dal.Abstractions.Common;
using Dal.Abstractions.Entities;
using Dal.Abstractions.Enums;
using Dal.Abstractions.Repositories;

namespace Core.Operations;

internal sealed class ReserveResourcesOperation(
    IResourceEstimator estimator,
    IResourceRepository resources,
    IReservationRepository reservations,
    IUnitOfWork unitOfWork)
    : IReserveResourcesOperation
{
    public async Task<ReservationOutcome> ExecuteAsync(
        Guid orderId,
        DateTimeOffset fromUtc,
        DateTimeOffset toUtc,
        CancellationToken cancellationToken)
    {
        var archiveDuration = toUtc - fromUtc;
        var bundle = estimator.Estimate(archiveDuration);
        var estimates = bundle.Resources;

        await using var tx = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var available = await resources.GetAllAsync(cancellationToken);
        var byType = available.ToDictionary(x => x.Type);

        foreach (var estimate in estimates)
        {
            if (!byType.TryGetValue(estimate.Type, out var res))
            {
                return new ReservationOutcome
                {
                    Success = false,
                    Estimates = estimates,
                    EstimatedProcessingDuration = bundle.EstimatedProcessingDuration,
                    FailureReason = $"Resource {estimate.Type} is not registered.",
                };
            }

            if (res.TotalCapacity - res.ReservedAmount < estimate.Amount)
            {
                return new ReservationOutcome
                {
                    Success = false,
                    Estimates = estimates,
                    EstimatedProcessingDuration = bundle.EstimatedProcessingDuration,
                    FailureReason = $"Insufficient {estimate.Type}: requested {estimate.Amount} {estimate.Unit}, available {res.TotalCapacity - res.ReservedAmount} {res.Unit}.",
                };
            }
        }

        var reservedAt = DateTimeOffset.UtcNow;
        var holdUntil = reservedAt + bundle.EstimatedProcessingDuration;
        var newReservations = new List<Reservation>(estimates.Count);

        foreach (var estimate in estimates)
        {
            var res = byType[estimate.Type];
            res.ReservedAmount += estimate.Amount;

            newReservations.Add(new Reservation
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ResourceType = estimate.Type,
                Amount = estimate.Amount,
                ReservedAtUtc = reservedAt,
                HoldUntilUtc = holdUntil,
                Status = ReservationStatus.Active,
            });
        }

        await reservations.AddRangeAsync(newReservations, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        return new ReservationOutcome
        {
            Success = true,
            Estimates = estimates,
            EstimatedProcessingDuration = bundle.EstimatedProcessingDuration,
            HoldUntilUtc = holdUntil,
        };
    }
}
