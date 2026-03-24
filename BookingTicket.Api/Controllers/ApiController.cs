using BookingTicket.Domain.Common.Results;

using Microsoft.AspNetCore.Mvc;

namespace BookingTicket.Api.Controllers;

[ApiController]
public class ApiController : ControllerBase
{
    protected ActionResult Problem(List<Error> errors)
    {
        if (errors.Count is 0)
        {
            var unexpected = Error.Unexpected(description: "Unexpected error.");
            Result<object?> response = unexpected;
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        if (errors.All(error => error.Type == ErrorKind.Validation))
        {
            return ValidationProblem(errors);
        }

        return Problem(errors[0]);
    }

    private ObjectResult Problem(Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorKind.Conflict => StatusCodes.Status409Conflict,
            ErrorKind.Validation => StatusCodes.Status400BadRequest,
            ErrorKind.NotFound => StatusCodes.Status404NotFound,
            ErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorKind.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError,
        };

        Result<object?> response = error;
        return StatusCode(statusCode, response);
    }

    private ActionResult ValidationProblem(List<Error> errors)
    {
        Result<object?> response = errors;
        return StatusCode(StatusCodes.Status400BadRequest, response);
    }
}
