using BookingRoom.Application.Features.Rooms.Dtos;
using BookingRoom.Domain.Common.Results;
using MediatR;
namespace BookingRoom.Application.Features.Rooms.Queries.GetRoomById;

public sealed record GetRoomByIdQuery
(Guid Id):IRequest<Result<RoomDto>>;