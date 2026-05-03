using Dal.Abstractions.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class AuditController(IAuditRepository repository)
    : ControllerBase
{
    [HttpGet("ByOrder/{orderId}")]
    public async Task<IActionResult> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var entries = await repository.GetByOrderIdAsync(orderId, cancellationToken);
        return Ok(entries);
    }
}