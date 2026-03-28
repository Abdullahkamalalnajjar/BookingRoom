using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Application.Features.Bookings.Dtos;
using BookingRoom.Application.Features.Bookings.Mapper;
using BookingRoom.Domain.Bookings;
using BookingRoom.Domain.Common.Results;
using BookingRoom.Domain.Rooms;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingRoom.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommandHandler(IAppDbContext context, IUser user, IIdentityService identityService, ILogger<CreateBookingCommandHandler> logger) :
    IRequestHandler<CreateBookingCommand, Result<BookingDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly IUser _user = user;
    private readonly IIdentityService _identityService = identityService;
    private readonly ILogger<CreateBookingCommandHandler> _logger = logger;

    public async Task<Result<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_user.Id))
        {
            _logger.LogWarning("Create booking failed. Current user id is missing.");
            return BookingErrors.UserRequired;
        }

        var room = await _context.Rooms
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == request.RoomId, cancellationToken);

        if (room is null)
        {
            _logger.LogInformation("Create booking failed. Room not found. RoomId={RoomId}", request.RoomId);
            return RoomErrors.RoomNotFound;
        }

        var reserveResult = room.ReserveSeats(request.Seats);
        if (reserveResult.IsError)
        {
            _logger.LogInformation(
                "Create booking failed. RoomId={RoomId}, Seats={Seats}. Errors={Errors}",
                request.RoomId,
                request.Seats,
                reserveResult.Errors);

            return reserveResult.Errors;
        }

        var createResult = Booking.Create(Guid.NewGuid(), _user.Id, request.RoomId, request.Seats);
        if (createResult.IsError)
        {
            _logger.LogWarning(
                "Create booking failed. RoomId={RoomId}, Seats={Seats}. Errors={Errors}",
                request.RoomId,
                request.Seats,
                createResult.Errors);
            return createResult.Errors;
        }

        var booking = createResult.Value;

        // Allow the client to set an initial status (still validated separately).
        var updateResult = booking.Update(request.Seats, request.Status);
        if (updateResult.IsError)
        {
            _logger.LogWarning(
                "Create booking failed while applying status. Seats={Seats}, Status={Status}. Errors={Errors}",
                request.Seats,
                request.Status,
                updateResult.Errors);

            return updateResult.Errors;
        }

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync(cancellationToken);

        var userName = await _identityService.GetUserNameAsync(booking.UserId);
        return booking.ToDo(room.Name, userName);
    }
}
