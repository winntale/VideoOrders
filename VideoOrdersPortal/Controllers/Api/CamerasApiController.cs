using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoOrdersPortal.Clients;

namespace VideoOrdersPortal.Controllers.Api;

[ApiController]
[Authorize]
[Route("api/cameras")]
public sealed class CamerasApiController(ProcessingSystemClient processing) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var cameras = await processing.ListCamerasAsync(ct);
        return Ok(cameras);
    }
}
