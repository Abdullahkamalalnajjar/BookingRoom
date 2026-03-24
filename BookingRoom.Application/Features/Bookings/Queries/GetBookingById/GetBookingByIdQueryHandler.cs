using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Application.Features.Bookings.Dtos;
using BookingRoom.Application.Features.Bookings.Mapper;
using BookingRoom.Domain.Bookings;
using BookingRoom.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingRoom.Application.Features.Bookings.Queries.GetBookingById;

public sealed class GetBookingByIdQueryHandler(IAppDbContext context, ILogger<GetBookingByIdQueryHandler> logger) :
    IRequestHandler<GetBookingQuery, Result<BookingDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<GetBookingByIdQueryHandler> _logger = logger;

    public async Task<Result<BookingDto>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        var exist = await _context.Bookings
            .AsNoTracking()
            .Include(x => x.Room)
            .FirstOrDefaultAsync(x => x.Id == request.id, cancellationToken);

        if (exist is null)
        {
            _logger.LogInformation("Booking not found. Id={Id}", request.id);
            return BookingErrors.BookingNotFound;
        }

        return exist.ToDo();
    }
}
