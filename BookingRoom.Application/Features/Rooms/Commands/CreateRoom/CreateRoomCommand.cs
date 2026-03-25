using BookingRoom.Application.Features.Rooms.Dtos;
using BookingRoom.Domain.Common.Results;
using MediatR;

namespace BookingRoom.Application.Features.Rooms.Commands.CreateRoom;

public sealed record CreateRoomCommand
(
string Name ,
int SeatCapacity
    ):IRequest<Result<RoomDto>>;

