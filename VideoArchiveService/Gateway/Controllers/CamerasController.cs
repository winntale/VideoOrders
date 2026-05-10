using Core.Abstractions.Operations;
using Gateway.Extensions;
using Gateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public sealed class CamerasController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CameraDto>>> ListAsync(
        [FromServices] IListCamerasOperation processor,
        CancellationToken cancellationToken)
    {
        var result = await processor.ExecuteAsync(cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }

        var response = result.Value
            .Select(c => new CameraDto
            {
                Id = c.Id,
                Name = c.Name,
                IsActive = c.IsActive,
                Segments = c.Segments
                    .Select(s => new SegmentRangeDto { FromUtc = s.FromUtc, ToUtc = s.ToUtc })
                    .ToArray()
            })
            .ToArray();

        return Ok(response);
    }
}
