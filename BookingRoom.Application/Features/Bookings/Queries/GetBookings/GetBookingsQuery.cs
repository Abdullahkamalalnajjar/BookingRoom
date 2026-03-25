using BookingRoom.Application.Features.Bookings.Dtos;
using BookingRoom.Domain.Common.Results;
using MediatR;

namespace BookingRoom.Application.Features.Bookings.Queries.GetBookings;

public sealed record GetBookingsQuery() : IRequest<Result<List<BookingDto>>>;
