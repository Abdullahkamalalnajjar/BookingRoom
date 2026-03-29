using BookingRoom.Application.Features.Rooms.Dtos;
using BookingRoom.Domain.Common.Results;
using MediatR;

namespace BookingRoom.Application.Features.Rooms.Commands.UpdateRoom;


public sealed record UpdateRoomCommand
(
    Guid RoomId,
    string Name ,
    int SeatCapacity ,
    int AvailableSeats,
    decimal SeatPrice
):IRequest<Result<Updated>>;
