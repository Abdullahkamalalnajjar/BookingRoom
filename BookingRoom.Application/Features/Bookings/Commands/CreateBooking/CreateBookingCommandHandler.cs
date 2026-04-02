using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Application.Common.Notifications;
using BookingRoom.Application.Features.Bookings.Dtos;
using BookingRoom.Application.Features.Bookings.Mapper;
using BookingRoom.Domain.Bookings;
using BookingRoom.Domain.Common.Results;
using BookingRoom.Domain.Rooms;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingRoom.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommandHandler(
    IAppDbContext context,
    IUser user,
    IIdentityService identityService,
    INotificationService notificationService,
    ILogger<CreateBookingCommandHandler> logger) :
    IRequestHandler<CreateBookingCommand, Result<BookingDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly IUser _user = user;
    private readonly IIdentityService _identityService = identityService;
    private readonly INotificationService _notificationService = notificationService;
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

        var createResult = Booking.Create(Guid.NewGuid(), _user.Id, request.RoomId, request.Seats, room.SeatPrice);
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
        var updateResult = booking.Update(request.Seats, request.Status, room.SeatPrice);
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

        var userResult = await _identityService.GetUserByIdAsync(booking.UserId);
        var userName = userResult.IsError
            ? await _identityService.GetUserNameAsync(booking.UserId)
            : userResult.Value.Email;

        if (!userResult.IsError)
        {
            try
            {
                var email = BookingEmailTemplates.CreateBookingCreated(
                    booking.Id,
                    room.Name,
                    booking.Seats,
                    booking.TotalPrice);

                await _notificationService.SendEmailAsync(
                    userResult.Value.Email,
                    email.Subject,
                    email.Body,
                    email.IsBodyHtml,
                    cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(
                    exception,
                    "Booking {BookingId} was created but the confirmation email could not be sent.",
                    booking.Id);
            }
        }
        else
        {
            _logger.LogWarning(
                "Booking {BookingId} was created but no user email was available for notification.",
                booking.Id);
        }

        return booking.ToDo(room.Name, userName);
    }
}
