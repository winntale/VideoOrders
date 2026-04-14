using AutoMapper;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Gateway.Extensions;
using Gateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public sealed class ArchiveValidationController(IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ArchiveAvailabilityResponseDto>> ValidateAsync(
        [FromServices] IValidateArchiveAvailabilityOperation processor,
        [FromBody] ValidateArchiveAvailabilityRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        var operationModel = mapper.Map<ValidateArchiveAvailabilityOperationModel>(requestDto);

        var result = await processor.ExecuteAsync(operationModel, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }

        var response = mapper.Map<ArchiveAvailabilityResponseDto>(result.Value);
        return Ok(response);
    }
}