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
            ErrorType.Forbidden  => StatusCodes.Status403Forbidden,
            ErrorType.Failure    => StatusCodes.Status500InternalServerError,
            _                    => StatusCodes.Status500InternalServerError
        };

        return new ObjectResult(new { error = error.Message }) { StatusCode = statusCode };
    }
}