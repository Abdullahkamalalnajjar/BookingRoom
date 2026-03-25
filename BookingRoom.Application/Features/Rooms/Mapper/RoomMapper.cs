using BookingRoom.Application.Features.Rooms.Dtos;
using BookingRoom.Domain.Rooms;

namespace BookingRoom.Application.Features.Rooms.Mapper;

public static class RoomsMapper
{
    public static RoomDto ToDto(this Room room)
    {
        ArgumentNullException.ThrowIfNull(room);
        return new RoomDto(room.Id,room.Name,room.SeatCapacity,room.AvailableSeats);
    }
    
    public static List<RoomDto> ToDtos(this List<Room> rooms)
    {
        ArgumentNullException.ThrowIfNull(rooms);
        return [..rooms.Select(r => ToDto(r)!)];
    }
}