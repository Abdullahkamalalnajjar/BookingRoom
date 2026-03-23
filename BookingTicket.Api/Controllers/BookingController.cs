using BookingTicket.Application.Features.Bookings.Commands.CreateBooking;
using BookingTicket.Application.Features.Bookings.Dtos;
using BookingTicket.Application.Features.Bookings.Queries.GetBookingById;
using BookingTicket.Application.Features.Bookings.Queries.GetBookings;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BookingTicket.Api.Controllers;

[Route("api/bookings")]
public sealed class BookingController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetBookingQuery(id), cancellationToken);

        return result.Match(
            onValue: booking => Ok(booking),
            onError: errors => Problem(errors));
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBookings(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetBookingsQuery(), cancellationToken);

        return result.Match(
            onValue: bookings => Ok(bookings),
            onError: errors => Problem(errors));
    }

    [HttpPost]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [FromBody] CreateBookingCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            onValue: booking => CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking),
            onError: errors => Problem(errors));
    }
}
