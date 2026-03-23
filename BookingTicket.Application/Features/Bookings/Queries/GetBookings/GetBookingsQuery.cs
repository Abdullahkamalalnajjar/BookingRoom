using BookingTicket.Application.Features.Bookings.Dtos;
using BookingTicket.Domain.Common.Results;
using MediatR;

namespace BookingTicket.Application.Features.Bookings.Queries.GetBookings;

public sealed class GetBookingsQuery : IRequest<Result<List<BookingDto>>>
{
    
}