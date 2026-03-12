using Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Extensions;

public static class ResponseExtensions
{
    public static ActionResult ToResponse(this Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound   => StatusCodes.Status404NotFound,
            ErrorType.Conflict   => StatusCodes.Status409Conflict,
            ErrorType.Failure    => StatusCodes.Status500InternalServerError,
            ErrorType.Forbidden  => StatusCodes.Status403Forbidden, 
            _                    => StatusCodes.Status500InternalServerError
        };

        var response = new JsonResult(new { error = error.Message });
        return new ObjectResult(response) { StatusCode = statusCode };
    }
}