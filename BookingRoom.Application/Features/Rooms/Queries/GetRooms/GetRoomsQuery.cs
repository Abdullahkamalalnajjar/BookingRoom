using BookingRoom.Application.Features.Rooms.Dtos;
using BookingRoom.Domain.Common.Results;
using MediatR;

namespace BookingRoom.Application.Features.Rooms.Queries.GetRooms;

public sealed record GetRoomsQuery
    () : IRequest<Result<List<RoomDto>>>;
