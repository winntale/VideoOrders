using Dal.Abstractions.Entities;

namespace Dal.Abstractions.Repositories;

public interface IReservationRepository
{
    Task<IReadOnlyList<Reservation>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Reservation>> GetActiveByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);

    Task AddRangeAsync(IEnumerable<Reservation> reservations, CancellationToken cancellationToken);
}
