namespace BookingRoom.Application.Features.Rooms.Dtos;

public sealed record  RoomDto
(
    Guid RoomId,
    string Name,
    int SeatCapacity,
    int AvailableSeats,
    decimal SeatPrice
);
