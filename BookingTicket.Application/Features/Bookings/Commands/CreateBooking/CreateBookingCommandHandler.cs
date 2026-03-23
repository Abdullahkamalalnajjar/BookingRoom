using BookingTicket.Application.Common.Interfaces;
using BookingTicket.Application.Features.Bookings.Dtos;
using BookingTicket.Application.Features.Bookings.Mapper;
using BookingTicket.Domain.Bookings;
using BookingTicket.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookingTicket.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommandHandler(IAppDbContext context, ILogger<CreateBookingCommandHandler> logger) :
    IRequestHandler<CreateBookingCommand, Result<BookingDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<CreateBookingCommandHandler> _logger = logger;

    public async Task<Result<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var createResult = Booking.Create(Guid.NewGuid(), request.seats);
        if (createResult.IsError)
        {
            _logger.LogWarning("Create booking failed. Seats={Seats}. Errors={Errors}", request.seats, createResult.Errors);
            return createResult.Errors;
        }

        var booking = createResult.Value;

        // Allow the client to set an initial status (still validated separately).
        var updateResult = booking.Update(request.seats, request.status);
        if (updateResult.IsError)
        {
            _logger.LogWarning(
                "Create booking failed while applying status. Seats={Seats}, Status={Status}. Errors={Errors}",
                request.seats,
                request.status,
                updateResult.Errors);

            return updateResult.Errors;
        }

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync(cancellationToken);

        return booking.ToDo();
    }
}
