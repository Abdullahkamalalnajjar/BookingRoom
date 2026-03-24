using BookingTicket.Application.Features.Bookings.Dtos;
using BookingTicket.Domain.Common.Results;
using BookingTicket.Domain.Enums;
using MediatR;

namespace BookingTicket.Application.Features.Bookings.Commands.UpdateBooking;

public sealed record UpdateBookingCommand
(Guid Id, int Seats, BookingStatus Status, Guid RoomId) : IRequest<Result<BookingDto>>;
