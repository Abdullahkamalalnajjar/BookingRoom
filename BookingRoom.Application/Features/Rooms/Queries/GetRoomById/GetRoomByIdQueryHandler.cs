using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Application.Features.Rooms.Dtos;
using BookingRoom.Application.Features.Rooms.Mapper;
using BookingRoom.Domain.Common.Results;
using BookingRoom.Domain.Rooms;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingRoom.Application.Features.Rooms.Queries.GetRoomById;

public sealed class GetRoomByIdQueryHandle(IAppDbContext context): IRequestHandler<GetRoomByIdQuery, Result<RoomDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<RoomDto>> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
    {
        var room = await _context.Rooms.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (room == null)
        {
            return RoomErrors.RoomNotFound;
        }
        var roomDto = room.ToDto();
        return (roomDto);
    }
}
