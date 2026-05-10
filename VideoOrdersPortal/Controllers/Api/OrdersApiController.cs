using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoOrdersPortal.Auth;
using VideoOrdersPortal.Clients;
using VideoOrdersPortal.Models;

namespace VideoOrdersPortal.Controllers.Api;

[ApiController]
[Authorize]
[Route("api/orders")]
public sealed class OrdersApiController(
    OrderServiceClient orders,
    UserServiceClient users,
    VideoArchiveServiceClient archive) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var userId = HttpContext.GetUserId();
        if (userId is null) return Unauthorized();

        var ordersTask = orders.ListByUserAsync(userId.Value, ct);
        var camerasTask = archive.ListCamerasAsync(ct);
        await Task.WhenAll(ordersTask, camerasTask);

        var nameById = (await camerasTask).ToDictionary(x => x.Id, x => x.Name);

        var enriched = (await ordersTask)
            .Select(o => new OrderListItemDto(
                o.Id,
                o.UserId,
                o.CameraId,
                nameById.TryGetValue(o.CameraId, out var name) ? name : o.CameraId.ToString(),
                o.FromUtc,
                o.ToUtc,
                o.Status,
                o.FailureReason,
                o.ArchiveFile))
            .ToList();

        return Ok(enriched);
    }

    [HttpPost("validate-access")]
    public async Task<IActionResult> ValidateAccess([FromBody] ValidateAccessRequest request, CancellationToken ct)
    {
        var userId = HttpContext.GetUserId();
        if (userId is null) return Unauthorized();

        var result = await users.ValidateAccessAsync(userId.Value, request.CameraId, ct);
        return Ok(result);
    }

    [HttpPost("validate-archive")]
    public async Task<IActionResult> ValidateArchive([FromBody] ValidateArchiveRequest request, CancellationToken ct)
    {
        var result = await archive.ValidateAvailabilityAsync(request.CameraId, request.FromUtc, request.ToUtc, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request, CancellationToken ct)
    {
        var userId = HttpContext.GetUserId();
        if (userId is null) return Unauthorized();

        var (response, order) = await orders.CreateAsync(
            userId.Value, request.CameraId, request.FromUtc, request.ToUtc, ct);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct);
            return StatusCode((int)response.StatusCode, new { error = body });
        }

        return Ok(order);
    }

    [HttpGet("{orderId:guid}/download")]
    public Task<IActionResult> Download(Guid orderId, CancellationToken ct) =>
        ProxyAsync($"/Orders/{orderId}/download", ct);

    [HttpGet("{orderId:guid}/stream")]
    public Task<IActionResult> Stream(Guid orderId, CancellationToken ct) =>
        ProxyAsync($"/Orders/{orderId}/stream", ct);

    private async Task<IActionResult> ProxyAsync(string path, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);

        if (Request.Headers.TryGetValue("Range", out var range))
        {
            request.Headers.TryAddWithoutValidation("Range", range.ToArray());
        }

        var upstream = await orders.Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);

        Response.StatusCode = (int)upstream.StatusCode;

        foreach (var header in upstream.Headers)
        {
            Response.Headers[header.Key] = header.Value.ToArray();
        }
        foreach (var header in upstream.Content.Headers)
        {
            Response.Headers[header.Key] = header.Value.ToArray();
        }
        Response.Headers.Remove("transfer-encoding");

        await upstream.Content.CopyToAsync(Response.Body, ct);
        return new EmptyResult();
    }
}
