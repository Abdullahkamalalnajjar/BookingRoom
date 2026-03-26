using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Domain.Common.Results;
using BookingRoom.Domain.Rooms;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookingRoom.Application.Features.Rooms.Commands.UpdateRoom;

public class UpdateRoomCommandHandler(IAppDbContext context):
    IRequestHandler<UpdateRoomCommand ,Result<Updated>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<Updated>> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _context.Rooms
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == request.RoomId, cancellationToken);

        if (room is null)
        {
            return RoomErrors.RoomNotFound;
        }

        var normalizedName = request.Name?.Trim() ?? string.Empty;
        var roomNameExists = await _context.Rooms
            .AsNoTracking()
            .AnyAsync(x => x.Id != request.RoomId && x.Name == normalizedName, cancellationToken);

        if (roomNameExists)
        {
            return RoomErrors.RoomNameIsExist;
        }

        var updateResult = room.Update(normalizedName, request.SeatCapacity, request.AvailableSeats);
        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
