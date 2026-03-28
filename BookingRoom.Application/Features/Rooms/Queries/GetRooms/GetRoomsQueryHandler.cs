using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Application.Features.Rooms.Dtos;
using BookingRoom.Application.Features.Rooms.Mapper;
using BookingRoom.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookingRoom.Application.Features.Rooms.Queries.GetRooms;

public sealed class GetRoomsQueryHandler(IAppDbContext context, ILogger<GetRoomsQueryHandler> logger)
    : IRequestHandler<GetRoomsQuery, Result<List<RoomDto>>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<GetRoomsQueryHandler> _logger = logger;

    public async Task<Result<List<RoomDto>>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching rooms from DB");

        var rooms = await _context.Rooms
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return rooms.ToDtos();
    }
}
