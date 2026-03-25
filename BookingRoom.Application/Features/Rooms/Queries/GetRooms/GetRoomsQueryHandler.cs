using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Application.Features.Rooms.Dtos;
using BookingRoom.Application.Features.Rooms.Mapper;
using BookingRoom.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingRoom.Application.Features.Rooms.Queries.GetRooms;

public sealed class GetRoomsQueryHandler(IAppDbContext context)
    : IRequestHandler<GetRoomsQuery, Result<List<RoomDto>>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<List<RoomDto>>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
    {
        var rooms = await _context.Rooms
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return rooms.ToDtos();
    }
}
