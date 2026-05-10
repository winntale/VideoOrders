using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoOrdersPortal.Clients;
using VideoOrdersPortal.Models;

namespace VideoOrdersPortal.Controllers.Api;

[ApiController]
[Authorize]
[Route("api/cameras")]
public sealed class CamerasApiController(
    ProcessingSystemClient processing,
    VideoArchiveServiceClient archive) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var filesTask = processing.ListCamerasAsync(ct);
        var catalogTask = archive.ListCamerasAsync(ct);
        await Task.WhenAll(filesTask, catalogTask);

        var catalogById = (await catalogTask).ToDictionary(x => x.Id);

        var result = (await filesTask)
            .Select(f =>
            {
                catalogById.TryGetValue(f.Id, out var meta);
                var name = meta?.Name ?? f.DisplayName;
                var segments = (meta?.Segments ?? Array.Empty<SegmentRangeDto>())
                    .OrderBy(s => s.FromUtc)
                    .Select(s => new SegmentRange(s.FromUtc, s.ToUtc))
                    .ToList();
                return new CameraDto(f.Id, name, f.FileSize, segments);
            })
            .ToList();

        return Ok(result);
    }
}
