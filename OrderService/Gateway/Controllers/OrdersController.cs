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

    [HttpGet("{orderId:guid}/download")]
    public async Task<IActionResult> DownloadAsync(
        [FromServices] IDownloadArchiveFileOperation processor,
        [FromRoute] Guid orderId,
        CancellationToken cancellationToken)
    {
        var model = new DownloadArchiveFileOperationModel { OrderId = orderId };

        var result = await processor.ExecuteAsync(model, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }

        var file = result.Value;

        return File(
            file.Stream,
            file.ContentType,
            file.FileName,
            enableRangeProcessing: true);
    }
    
    [HttpGet("{orderId:guid}/stream")]
    public async Task<IActionResult> StreamAsync(
        [FromServices] IStreamArchiveFileOperation processor,
        [FromRoute] Guid orderId,
        CancellationToken cancellationToken)
    {
        var model = new StreamArchiveFileOperationModel { OrderId = orderId };

        var result = await processor.ExecuteAsync(model, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.ToResponse();
        }

        var file = result.Value;

        var streamResult = new FileStreamResult(file.Stream, file.ContentType)
        {
            EnableRangeProcessing = true,
            FileDownloadName = file.FileName
        };

        return streamResult;
    }
}