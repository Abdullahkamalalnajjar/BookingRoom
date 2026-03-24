using BookingTicket.Application.Common.Interfaces;
using BookingTicket.Application.Features.Bookings.Dtos;
using BookingTicket.Application.Features.Bookings.Mapper;
using BookingTicket.Domain.Common.Results;
using BookingTicket.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingTicket.Application.Features.Bookings.Queries.GetBookingByStatus;

public class GetBookingByStatusQueryHandler (IAppDbContext context):
    IRequestHandler<GetBookingByStatusQuery, Result<List<BookingDto>>>
{
    private readonly IAppDbContext _context = context;
    public async Task<Result<List<BookingDto>>> Handle(GetBookingByStatusQuery request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<BookingStatus>(request.status, ignoreCase: true, out var parsedStatus))
        {
            return Error.Validation("Booking_Status_Invalid", "الحالة المرسلة غير موجودة.");
        }

        var bookings = await _context.Bookings
            .AsNoTracking()
            .Include(x => x.Room)
            .Where(s => s.Status == parsedStatus)
            .ToListAsync(cancellationToken);

        return bookings.ToDtos();
    }
}
