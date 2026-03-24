using BookingTicket.Application.Common.Interfaces;
using BookingTicket.Application.Features.Bookings.Dtos;
using BookingTicket.Application.Features.Bookings.Mapper;
using BookingTicket.Domain.Common.Results;
using BookingTicket.Domain.Enums;
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
        IQueryable<BookingTicket.Domain.Bookings.Booking> bookingsQuery = _context.Bookings
            .AsNoTracking()
            .Include(x => x.Room);

        if (!string.IsNullOrWhiteSpace(request.status))
        {
            if (!Enum.TryParse<BookingStatus>(request.status, ignoreCase: true, out var parsedStatus) ||
                !Enum.IsDefined(typeof(BookingStatus), parsedStatus))
            {
                return Error.Validation("Booking_Status_Invalid", "الحالة المرسلة غير موجودة.");
            }

            bookingsQuery = bookingsQuery.Where(x => x.Status == parsedStatus);
        }
        
        var bookings = await bookingsQuery.ToListAsync(cancellationToken);
        return bookings.ToDtos();
    }
}
