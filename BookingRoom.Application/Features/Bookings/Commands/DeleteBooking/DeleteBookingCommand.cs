using BookingRoom.Domain.Common.Results;
using MediatR;

namespace BookingRoom.Application.Features.Bookings.Commands.DeleteBooking;

public sealed record DeleteBookingCommand
(Guid Id):IRequest<Result<Deleted>>;