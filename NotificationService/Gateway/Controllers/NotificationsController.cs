using Dal.Abstractions.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class NotificationsController(INotificationRepository repository) : ControllerBase
{
    [HttpGet("ByOrder/{orderId:guid}")]
    public async Task<IActionResult> GetByOrderAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var notifications = await repository.GetByOrderIdAsync(orderId, cancellationToken);
        return Ok(notifications);
    }

    [HttpGet("ByOrders")]
    public async Task<IActionResult> GetByOrdersAsync(
        [FromQuery(Name = "orderIds")] Guid[]? orderIds,
        CancellationToken cancellationToken)
    {
        var ids = orderIds is null || orderIds.Length == 0
            ? Array.Empty<Guid>()
            : orderIds;

        var notifications = await repository.GetByOrderIdsAsync(ids, cancellationToken);
        return Ok(notifications);
    }
}
