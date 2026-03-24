using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Application.Features.Bookings.Dtos;
using BookingRoom.Application.Features.Bookings.Mapper;
using BookingRoom.Domain.Common.Results;
using BookingRoom.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingRoom.Application.Features.Bookings.Queries.GetBookingByStatus;

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
