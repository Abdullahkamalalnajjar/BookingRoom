using BookingRoom.Api.Extensions;
using BookingRoom.Domain.Common.Results;

using Microsoft.AspNetCore.Mvc;

namespace BookingRoom.Api.Controllers;

[ApiController]
public class ApiController : ControllerBase
{
    protected ActionResult Problem(List<Error> errors) => this.ToActionResult(errors);
}
