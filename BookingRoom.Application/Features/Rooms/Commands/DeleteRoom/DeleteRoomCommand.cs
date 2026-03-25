using BookingRoom.Domain.Common.Results;
using MediatR;

namespace BookingRoom.Application.Features.Rooms.Commands.DeleteRoom;

public sealed record DeleteRoomCommand
(
    Guid RoomId
  
):IRequest<Result<Deleted>>;
