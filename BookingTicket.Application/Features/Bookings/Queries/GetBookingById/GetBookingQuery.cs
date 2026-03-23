using BookingTicket.Application.Features.Bookings.Dtos;
using BookingTicket.Domain.Common.Results;
using MediatR;

namespace BookingTicket.Application.Features.Bookings.Queries.GetBookingById;

public sealed record GetBookingQuery(Guid id) : IRequest<Result<BookingDto>>;
