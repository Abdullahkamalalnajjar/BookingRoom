using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Application.Features.Rooms.Dtos;
using BookingRoom.Application.Features.Rooms.Mapper;
using BookingRoom.Domain.Common.Results;
using BookingRoom.Domain.Rooms;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingRoom.Application.Features.Rooms.Commands.CreateRoom;

public sealed class CreateRoomCommandHandler (IAppDbContext context):
    IRequestHandler<CreateRoomCommand,Result<RoomDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<RoomDto>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var isExist = await _context.Rooms.AsNoTracking().AnyAsync(x=>x.Name==request.Name, cancellationToken: cancellationToken);
        if (isExist)
        {
            return RoomErrors.RoomNameIsExist;
        }

        if (request.SeatCapacity <= 0)
        {
            return RoomErrors.CapacityInvalid;
        }
        var roomResult = Room.Create(Guid.NewGuid(), request.Name, request.SeatCapacity);

        if (roomResult.IsError)
        {
            return roomResult.Errors;
        }   
        _context.Rooms.Add(roomResult.Value);
        await _context.SaveChangesAsync(cancellationToken);
        return roomResult.Value.ToDto();
    }
}
