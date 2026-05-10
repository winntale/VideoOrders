using AutoMapper;
using Core.Abstractions.Operations;
using Gateway.Extensions;
using Gateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public sealed class CamerasController(IMapper mapper) : ControllerBase
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

        var response = mapper.Map<IReadOnlyList<CameraDto>>(result.Value);
        return Ok(response);
    }
}
