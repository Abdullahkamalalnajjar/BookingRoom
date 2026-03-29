using BookingRoom.Application.Features.Bookings.Commands.CreateBooking;
using BookingRoom.Application.Features.Bookings.Commands.CreateBookingCheckout;
using BookingRoom.Application.Features.Bookings.Dtos;
using BookingRoom.Application.Features.Bookings.Queries.GetBookingById;
using BookingRoom.Application.Features.Bookings.Queries.GetBookings;
using BookingRoom.Application.Common.Security;
using BookingRoom.Domain.Common.Results;
using Asp.Versioning;
using BookingRoom.Application.Features.Bookings.Commands.DeleteBooking;
using BookingRoom.Application.Features.Bookings.Commands.UpdateBooking;
using BookingRoom.Application.Features.Bookings.Queries.GetBookingByStatus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace BookingRoom.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/bookings")]
public sealed class BookingController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    #region GetById
    [HttpGet("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.BookingsRead)]
    [ProducesResponseType(typeof(Result<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetBookingQuery(id), cancellationToken);

        return result.Match(
            onValue: booking =>
            {
                Result<BookingDto> response = booking;
                return Ok(response);
            },
            onError: errors => Problem(errors));
    }
    #endregion

    #region Get Bookings
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.BookingsRead)]
    [ProducesResponseType(typeof(Result<List<BookingDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a list of bookings.")]
    [EndpointDescription("Returns all bookings associated with the current user.")]
    [EndpointName("GetBookings")]
    [MapToApiVersion("1.0")]
    [ProducesDefaultResponseType]
    [OutputCache(Duration = 60)]
    public async Task<IActionResult> GetBookings(
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetBookingsQuery(), cancellationToken);

        return result.Match(
            onValue: bookings =>
            {
                Result<List<BookingDto>> response = bookings;
                return Ok(response);
            },
            onError: errors => Problem(errors));
    }
    #endregion

    #region Get Bookings By Status
    [HttpGet("{status}")]
    [Authorize(Policy = AuthorizationPolicies.BookingsRead)]
    [ProducesResponseType(typeof(Result<List<BookingDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status500InternalServerError)]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetBookingsByStatus([FromRoute] string status, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetBookingByStatusQuery(status), cancellationToken);

        return result.Match(
            onValue: bookings =>
            {
                Result<List<BookingDto>> response = bookings;
                return Ok(response);
            },
            onError: errors => Problem(errors));
    }
    #endregion

    #region Create Booking
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.BookingsWrite)]
    [ProducesResponseType(typeof(Result<BookingDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        [FromBody] CreateBookingCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            onValue: booking =>
            {
                Result<BookingDto> response = booking;
                return CreatedAtAction(nameof(GetById), new { id = booking.Id }, response);
            },
            onError: Problem);
    }
    #endregion

    #region Create Booking Checkout
    [HttpPost("checkout")]
    [Authorize(Policy = AuthorizationPolicies.BookingsWrite)]
    [ProducesResponseType(typeof(Result<BookingCheckoutDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCheckout(
        [FromBody] CreateBookingCheckoutCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.Match(
            onValue: checkout =>
            {
                Result<BookingCheckoutDto> response = checkout;
                return CreatedAtAction(nameof(GetById), new { id = checkout.Booking.Id }, response);
            },
            onError: Problem);
    }
    #endregion

    #region Update Booking
    [HttpPut]
    [Authorize(Policy = AuthorizationPolicies.BookingsWrite)]
   public async Task<IActionResult> Update([FromBody] UpdateBookingCommand command,CancellationToken ctx)
   {
       var result = await _sender.Send(command, ctx);
       return result.Match(
           onValue: Ok,
           onError: Problem);
   }
    #endregion

    #region Delete
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.BookingsWrite)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new DeleteBookingCommand(id), cancellationToken);
        return result.Match(_ => NoContent(),
            Problem);
    }
    #endregion
}
