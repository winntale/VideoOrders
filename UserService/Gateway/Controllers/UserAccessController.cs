using AutoMapper;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Gateway.Extensions;
using Gateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public sealed class UserAccessController(
    IMapper mapper)
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<UserAccessValidationResponseDto>> ValidateAsync(
        [FromServices] IValidateUserAccessOperation processor,
        [FromBody] ValidateUserAccessDto requestModel,
        CancellationToken cancellationToken)
    {
        var operationModel = mapper.Map<ValidateUserAccessOperationModel>(requestModel);

        var result = await processor.ExecuteAsync(operationModel, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }

        var response = mapper.Map<UserAccessValidationResponseDto>(result.Value);
        return Ok(response);
    }
}