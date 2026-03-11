using AutoMapper;
using Core.Abstractions;
using Core.Abstractions.OperationModels;
using Core.Abstractions.Operations;
using Gateway.Extensions;
using Gateway.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public sealed class OrdersController(IMapper mapper)
    : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderResponseDto>> CreateAsync(
        [FromServices] ICreateOrderOperation processor,
        [FromBody] CreateOrderDto requestModel,
        CancellationToken cancellationToken)
    {
        var operationModel = mapper.Map<CreateOrderOperationModel>(requestModel);

        var result = await processor.ExecuteAsync(operationModel, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }

        var response = mapper.Map<OrderResponseDto>(result.Value);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponseDto>> GetByIdAsync(
        [FromServices] IGetOrderByIdOperation processor,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var operationModel = new GetOrderByIdOperationModel { Id = id };

        var result = await processor.ExecuteAsync(operationModel, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }

        var response = mapper.Map<OrderResponseDto>(result.Value);
        return Ok(response);
    }
    
    // TODO: ChangeOrderStatus Action
}