using BookingTicket.Application.Features.Bookings.Dtos;
using BookingTicket.Domain.Common.Results;
using BookingTicket.Domain.Enums;
using MediatR;

namespace BookingTicket.Application.Features.Bookings.Commands.CreateBooking;

public sealed record CreateBookingCommand
(
    int seats,
    BookingStatus status
    ):IRequest<Result<BookingDto>>;