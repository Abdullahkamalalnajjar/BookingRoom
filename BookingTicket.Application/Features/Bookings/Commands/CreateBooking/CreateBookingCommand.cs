using BookingTicket.Application.Features.Bookings.Dtos;
using BookingTicket.Domain.Common.Results;
using BookingTicket.Domain.Enums;
using MediatR;

namespace BookingTicket.Application.Features.Bookings.Commands.CreateBooking;

public sealed record CreateBookingCommand
(
    Guid RoomId,
    int Seats,
    BookingStatus Status
    ):IRequest<Result<BookingDto>>;
