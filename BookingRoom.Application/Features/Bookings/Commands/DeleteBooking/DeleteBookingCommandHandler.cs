using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Domain.Bookings;
using BookingRoom.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingRoom.Application.Features.Bookings.Commands.DeleteBooking;

public sealed class DeleteBookingCommandHandler(
    IAppDbContext context):
    IRequestHandler<DeleteBookingCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Deleted>> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings.AsTracking().Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.Id == request.Id,cancellationToken);
        if (booking == null)
        {
            return BookingErrors.BookingNotFound;
        }
        booking.Room?.ReleaseSeats(booking.Seats);
        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Deleted;
    }
}
