using BookingRoom.Application.Features.Bookings.Dtos;
using BookingRoom.Domain.Common.Results;
using BookingRoom.Domain.Enums;
using MediatR;

namespace BookingRoom.Application.Features.Bookings.Commands.UpdateBooking;

public sealed record UpdateBookingCommand
(Guid Id, int Seats, BookingStatus Status, Guid RoomId) : IRequest<Result<BookingDto>>;
