using BookingTicket.Application.Features.Bookings.Dtos;
using BookingTicket.Domain.Common.Results;
using BookingTicket.Domain.Enums;
using MediatR;

namespace BookingTicket.Application.Features.Bookings.Queries.GetBookingByStatus;

public sealed record GetBookingByStatusQuery(string? status)
    : IRequest<Result<List<BookingDto>>>;
