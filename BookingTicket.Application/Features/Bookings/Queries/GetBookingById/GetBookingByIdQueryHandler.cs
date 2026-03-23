using BookingTicket.Application.Common.Interfaces;
using BookingTicket.Application.Features.Bookings.Dtos;
using BookingTicket.Application.Features.Bookings.Mapper;
using BookingTicket.Domain.Bookings;
using BookingTicket.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingTicket.Application.Features.Bookings.Queries.GetBookingById;

public sealed class GetBookingByIdQueryHandler(IAppDbContext context, ILogger<GetBookingByIdQueryHandler> logger) :
    IRequestHandler<GetBookingQuery, Result<BookingDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<GetBookingByIdQueryHandler> _logger = logger;

    public async Task<Result<BookingDto>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        var exist = await _context.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.id, cancellationToken);

        if (exist is null)
        {
            _logger.LogInformation("Booking not found. Id={Id}", request.id);
            return BookingErrors.BookingNotFound;
        }

        return exist.ToDo();
    }
}
