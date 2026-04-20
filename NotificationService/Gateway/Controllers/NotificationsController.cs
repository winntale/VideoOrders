using Dal.Abstractions.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

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
}