using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoOrdersPortal.Auth;
using VideoOrdersPortal.Clients;
using VideoOrdersPortal.Models;

namespace VideoOrdersPortal.Controllers.Api;

[ApiController]
[Authorize]
[Route("api/notifications")]
public sealed class NotificationsApiController(
    OrderServiceClient orders,
    NotificationServiceClient notifications,
    VideoArchiveServiceClient archive) : ControllerBase
{
    private static readonly string[] TypeNames =
    {
        "OrderCompleted",
        "OrderFailed",
        "ResourceReservationFailed"
    };

    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var userId = HttpContext.GetUserId();
        if (userId is null) return Unauthorized();

        var ordersList = await orders.ListByUserAsync(userId.Value, ct);
        if (ordersList.Count == 0)
        {
            return Ok(Array.Empty<UserNotificationDto>());
        }

        var orderIds = ordersList.Select(o => o.Id).ToArray();
        var cameraIdByOrder = ordersList.ToDictionary(o => o.Id, o => o.CameraId);

        var notificationsTask = notifications.ListByOrderIdsAsync(orderIds, ct);
        var camerasTask = archive.ListCamerasAsync(ct);
        await Task.WhenAll(notificationsTask, camerasTask);

        var nameById = (await camerasTask).ToDictionary(x => x.Id, x => x.Name);

        var enriched = (await notificationsTask)
            .OrderByDescending(n => n.CreatedAtUtc)
            .Select(n =>
            {
                cameraIdByOrder.TryGetValue(n.OrderId, out var cameraId);
                var cameraName = nameById.TryGetValue(cameraId, out var name)
                    ? name
                    : (cameraId == Guid.Empty ? string.Empty : cameraId.ToString());

                return new UserNotificationDto(
                    n.Id,
                    n.OrderId,
                    cameraId,
                    cameraName,
                    n.Type >= 0 && n.Type < TypeNames.Length ? TypeNames[n.Type] : n.Type.ToString(),
                    n.Message,
                    n.CreatedAtUtc);
            })
            .ToList();

        return Ok(enriched);
    }
}
