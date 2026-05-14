using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoOrdersPortal.Auth;
using VideoOrdersPortal.Clients;
using VideoOrdersPortal.Models;

namespace VideoOrdersPortal.Controllers.Api;

[ApiController]
[Authorize]
[Route("api/cameras")]
public sealed class CamerasApiController(
    ProcessingSystemClient processing,
    VideoArchiveServiceClient archive,
    UserServiceClient users) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var userId = HttpContext.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var filesTask = processing.ListCamerasAsync(ct);
        var catalogTask = archive.ListCamerasAsync(ct);
        var accessibleTask = users.GetAccessibleCameraIdsAsync(userId.Value, ct);
        await Task.WhenAll(filesTask, catalogTask, accessibleTask);

        var allowed = (await accessibleTask).ToHashSet();
        var catalogById = (await catalogTask).ToDictionary(x => x.Id);

        var result = (await filesTask)
            .Where(f => allowed.Contains(f.Id))
            .Select(f =>
            {
                catalogById.TryGetValue(f.Id, out var meta);
                var name = meta?.Name ?? f.DisplayName;
                var isActive = meta?.IsActive ?? true;
                var segments = (meta?.Segments ?? Array.Empty<SegmentRangeDto>())
                    .OrderBy(s => s.FromUtc)
                    .Select(s => new SegmentRange(s.FromUtc, s.ToUtc))
                    .ToList();
                return new CameraDto(f.Id, name, f.FileSize, isActive, segments);
            })
            .OrderBy(c => c.IsActive ? 0 : 1)
            .ThenBy(c => c.Name)
            .ToList();

        return Ok(result);
    }
}
