using BookingRoom.Application.Features.Bookings.Dtos;
using BookingRoom.Domain.Common.Results;
using BookingRoom.Domain.Enums;
using MediatR;

namespace BookingRoom.Application.Features.Bookings.Queries.GetBookingByStatus;

public sealed record GetBookingByStatusQuery(string? status)
    : IRequest<Result<List<BookingDto>>>;
