using BookingRoom.Application.Features.Bookings.Dtos;
using BookingRoom.Domain.Common.Results;
using BookingRoom.Domain.Enums;
using MediatR;

namespace BookingRoom.Application.Features.Bookings.Commands.CreateBooking;

public sealed record CreateBookingCommand
(
    Guid RoomId,
    int Seats,
    BookingStatus Status
    ):IRequest<Result<BookingDto>>;
