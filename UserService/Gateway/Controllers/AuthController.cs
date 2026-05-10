using AutoMapper;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Gateway.Extensions;
using Gateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public sealed class AuthController(IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AuthenticatedUserDto>> LoginAsync(
        [FromServices] ILoginUserOperation processor,
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        var operationModel = mapper.Map<LoginUserOperationModel>(request);

        var result = await processor.ExecuteAsync(operationModel, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }

        return Ok(mapper.Map<AuthenticatedUserDto>(result.Value));
    }

    [HttpPost]
    public async Task<ActionResult<AuthenticatedUserDto>> RegisterAsync(
        [FromServices] IRegisterUserOperation processor,
        [FromBody] RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        var operationModel = mapper.Map<RegisterUserOperationModel>(request);

        var result = await processor.ExecuteAsync(operationModel, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }

        return Ok(mapper.Map<AuthenticatedUserDto>(result.Value));
    }
}
