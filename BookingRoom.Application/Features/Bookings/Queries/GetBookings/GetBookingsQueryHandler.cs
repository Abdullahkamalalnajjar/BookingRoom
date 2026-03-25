using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Application.Features.Bookings.Dtos;
using BookingRoom.Application.Features.Bookings.Mapper;
using BookingRoom.Domain.Common.Results;
using BookingRoom.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingRoom.Application.Features.Bookings.Queries.GetBookings;

public class GetBookingsQueryHandler (IAppDbContext  context,ILogger<GetBookingsQueryHandler> logger)
: IRequestHandler<GetBookingsQuery, Result<List<BookingDto>>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<GetBookingsQueryHandler> _logger = logger;
    public async Task<Result<List<BookingDto>>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        IQueryable<BookingRoom.Domain.Bookings.Booking> bookingsQuery = _context.Bookings
            .AsNoTracking()
            .Include(x => x.Room);
        var bookings = await bookingsQuery.ToListAsync(cancellationToken);
        return bookings.ToDtos();
    }
}
