using BookingTicket.Application.Common.Interfaces;
using BookingTicket.Application.Features.Bookings.Dtos;
using BookingTicket.Application.Features.Bookings.Mapper;
using BookingTicket.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingTicket.Application.Features.Bookings.Queries.GetBookings;

public class GetBookingsQueryHandler (IAppDbContext  context,ILogger<GetBookingsQueryHandler> logger)
: IRequestHandler<GetBookingsQuery, Result<List<BookingDto>>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<GetBookingsQueryHandler> _logger = logger;
    public async Task<Result<List<BookingDto>>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        var bookings = await _context.Bookings.AsNoTracking().ToListAsync(cancellationToken);
        return bookings.ToDtos();
    }
}