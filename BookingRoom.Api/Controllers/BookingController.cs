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
using System.Text.Json;
using BookingRoom.Application.Features.Bookings.Queries.GetBookingsForMember;
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
    
    #region Get Booking for member
    [HttpGet("for-member")]
    [Authorize(Policy = AuthorizationPolicies.BookingsRead)]
    [ProducesResponseType(typeof(Result<List<BookingDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<object?>), StatusCodes.Status500InternalServerError)]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetForMember(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetBookingsForMemberQuery(),cancellationToken);
        return result.Match(
            onValue: bookings =>
            {
                Result<List<BookingDto>> response = bookings;
                return Ok(response);
            }, onError:Problem);
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
                checkout.PaymentPageUrl ??= Url.ActionLink(
                    nameof(PayPage),
                    values: new
                    {
                        bookingId = checkout.Booking.Id,
                        clientSecret = checkout.ClientSecret,
                        publicKey = checkout.PublicKey
                    });

                Result<BookingCheckoutDto> response = checkout;
                return CreatedAtAction(nameof(GetById), new { id = checkout.Booking.Id }, response);
            },
            onError: Problem);
    }
    #endregion

    #region Pay Page
    [HttpGet("pay/{bookingId:guid}")]
    [AllowAnonymous]
    [Produces("text/html")]
    public IActionResult PayPage(
        [FromRoute] Guid bookingId,
        [FromQuery] string clientSecret,
        [FromQuery] string publicKey)
    {
        if (string.IsNullOrWhiteSpace(clientSecret) || string.IsNullOrWhiteSpace(publicKey))
        {
            return BadRequest("Missing clientSecret or publicKey.");
        }

        var html = BuildPaymobCheckoutPage(bookingId, clientSecret, publicKey);
        return Content(html, "text/html; charset=utf-8");
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

    private static string BuildPaymobCheckoutPage(Guid bookingId, string clientSecret, string publicKey)
    {
        var serializedBookingId = JsonSerializer.Serialize(bookingId.ToString());
        var serializedClientSecret = JsonSerializer.Serialize(clientSecret);
        var serializedPublicKey = JsonSerializer.Serialize(publicKey);

        return $$"""
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Booking Payment</title>
  <style>
    :root {
      color-scheme: light;
      --bg: #f4efe7;
      --panel: #fffaf2;
      --text: #1d1d1b;
      --muted: #6d665c;
      --accent: #b85c38;
      --border: #e8dccb;
    }

    * { box-sizing: border-box; }

    body {
      margin: 0;
      min-height: 100vh;
      display: grid;
      place-items: center;
      background:
        radial-gradient(circle at top left, #f7c78f 0, transparent 28%),
        radial-gradient(circle at bottom right, #d2e7d0 0, transparent 30%),
        var(--bg);
      color: var(--text);
      font-family: Georgia, "Times New Roman", serif;
      padding: 24px;
    }

    main {
      width: min(100%, 680px);
      background: var(--panel);
      border: 1px solid var(--border);
      border-radius: 24px;
      padding: 32px;
      box-shadow: 0 18px 60px rgba(29, 29, 27, 0.12);
    }

    h1 {
      margin: 0 0 8px;
      font-size: clamp(2rem, 5vw, 3.2rem);
      line-height: 0.95;
    }

    p {
      margin: 0;
      color: var(--muted);
      line-height: 1.6;
    }

    .meta {
      margin-top: 20px;
      padding: 14px 16px;
      border-radius: 16px;
      background: #fff;
      border: 1px solid var(--border);
      font-size: 0.95rem;
    }

    .meta strong {
      color: var(--text);
    }

    #paymob-checkout {
      margin-top: 24px;
      min-height: 72px;
    }

    #status {
      margin-top: 18px;
      padding: 14px 16px;
      border-radius: 16px;
      background: #fff;
      border: 1px solid var(--border);
    }
  </style>
</head>
<body>
  <main>
    <h1>Complete Your Payment</h1>
    <p>Use the Paymob checkout button below to finish paying for your booking.</p>
    <div class="meta"><strong>Booking ID:</strong> <span id="booking-id"></span></div>
    <div id="paymob-checkout"></div>
    <div id="status">Preparing checkout...</div>
  </main>

  <script src="https://nextstagingenv.s3.amazonaws.com/js/v1/paymob.js"></script>
  <script>
    const bookingId = {{serializedBookingId}};
    const clientSecret = {{serializedClientSecret}};
    const publicKey = {{serializedPublicKey}};

    document.getElementById("booking-id").textContent = bookingId;

    try {
      Paymob(publicKey).checkoutButton(clientSecret).mount("#paymob-checkout");
      document.getElementById("status").textContent = "Checkout is ready. Click the payment button to continue.";
    } catch (error) {
      document.getElementById("status").textContent =
        "Unable to load Paymob checkout on this page. " + (error && error.message ? error.message : "");
    }
  </script>
</body>
</html>
""";
    }
}
