using BookingRoom.Application.Features.Bookings.Dtos;
using BookingRoom.Domain.Common.Results;
using MediatR;

namespace BookingRoom.Application.Features.Bookings.Queries.GetBookingById;

public sealed record GetBookingQuery(Guid id) : IRequest<Result<BookingDto>>;
