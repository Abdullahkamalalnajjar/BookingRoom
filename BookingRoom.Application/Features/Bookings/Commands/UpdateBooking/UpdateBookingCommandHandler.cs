using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Application.Features.Bookings.Dtos;
using BookingRoom.Application.Features.Bookings.Mapper;
using BookingRoom.Domain.Bookings;
using BookingRoom.Domain.Common.Results;
using BookingRoom.Domain.Rooms;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingRoom.Application.Features.Bookings.Commands.UpdateBooking;

public class UpdateBookingCommandHandler(IAppDbContext context, IIdentityService identityService)
:IRequestHandler<UpdateBookingCommand,Result<BookingDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly IIdentityService _identityService = identityService;

    public async Task<Result<BookingDto>> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings.AsTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (booking is null)
        {
            return BookingErrors.BookingNotFound;
        }

        var targetRoom = await _context.Rooms.AsTracking()
            .FirstOrDefaultAsync(x => x.Id == request.RoomId, cancellationToken: cancellationToken);

        if (targetRoom is null)
        {
            return RoomErrors.RoomNotFound;
        }

        if (booking.RoomId == request.RoomId)
        {
            var seatsDelta = request.Seats - booking.Seats;

            if (seatsDelta > 0)
            {
                var reserveResult = targetRoom.ReserveSeats(seatsDelta);
                if (reserveResult.IsError)
                {
                    return reserveResult.Errors;
                }
            }
            else if (seatsDelta < 0)
            {
                var releaseResult = targetRoom.ReleaseSeats(Math.Abs(seatsDelta));
                if (releaseResult.IsError)
                {
                    return releaseResult.Errors;
                }
            }
        }
        else
        {
            var currentRoom = await _context.Rooms.AsTracking()
                .FirstOrDefaultAsync(x => x.Id == booking.RoomId, cancellationToken: cancellationToken);

            if (currentRoom is null)
            {
                return RoomErrors.RoomNotFound;
            }

            var reserveTargetResult = targetRoom.ReserveSeats(request.Seats);
            if (reserveTargetResult.IsError)
            {
                return reserveTargetResult.Errors;
            }

            var releaseCurrentResult = currentRoom.ReleaseSeats(booking.Seats);
            if (releaseCurrentResult.IsError)
            {
                return releaseCurrentResult.Errors;
            }

            var changeRoomResult = booking.ChangeRoom(request.RoomId);
            if (changeRoomResult.IsError)
            {
                return changeRoomResult.Errors;
            }
        }

        var updateResult = booking.Update(request.Seats, request.Status);
        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var userName = await _identityService.GetUserNameAsync(booking.UserId);
        return booking.ToDo(targetRoom.Name, userName);
    }
}
