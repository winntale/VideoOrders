using Dal.Abstractions.Entities;
using Dal.Abstractions.Enums;
using Dal.Abstractions.Repositories;
using Dal.Context;
using Microsoft.EntityFrameworkCore;

namespace Dal.Repositories;

internal sealed class ReservationRepository(ResourceDbContext dbContext) : IReservationRepository
{
    public async Task<IReadOnlyList<Reservation>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await dbContext.Reservations
            .AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Reservation>> GetActiveByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
    {
        return await dbContext.Reservations
            .Where(x => x.OrderId == orderId && x.Status == ReservationStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public Task AddRangeAsync(IEnumerable<Reservation> reservations, CancellationToken cancellationToken)
    {
        return dbContext.Reservations.AddRangeAsync(reservations, cancellationToken);
    }
}
