using BookingTicket.Domain.Common.Results;

namespace BookingTicket.Domain.Rooms;

public static class RoomErrors
{
    public static Error RoomNotFound =>
        Error.NotFound("Room_Not_Found", "Room not found");

    public static Error NameRequired =>
        Error.Validation("Room_Name_Required", "Room name is required");

    public static Error CapacityInvalid =>
        Error.Validation("Room_Capacity_Invalid", "Room capacity must be greater than zero");

    public static Error InvalidSeatCount =>
        Error.Validation("Room_Seat_Count_Invalid", "Seat count must be greater than zero");

    public static Error NotEnoughAvailableSeats(int availableSeats) =>
        Error.Validation("Room_Not_Enough_Available_Seats", $"Not enough available seats. Available: {availableSeats}.");

    public static Error AvailableSeatsOverflow =>
        Error.Unexpected("Room_Available_Seats_Overflow", "Available seats cannot exceed room capacity.");
}
