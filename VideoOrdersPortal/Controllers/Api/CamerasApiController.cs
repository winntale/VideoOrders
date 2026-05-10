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

        var nameById = (await catalogTask).ToDictionary(x => x.Id, x => x.Name);

        var result = (await filesTask)
            .Select(f => new CameraDto(
                f.Id,
                nameById.TryGetValue(f.Id, out var name) ? name : f.DisplayName,
                f.FileSize))
            .ToList();

        return Ok(result);
    }
}
