using BookingRoom.Domain.Common.Results;

namespace BookingRoom.Domain.Rooms;

public static class RoomErrors
{
    public static Error CreateRoomError => Error.Failure("Room_Create_Error", "Room creation error");
    public static Error RoomNotFound =>
        Error.NotFound("Room_Not_Found", "Room not found");
    public static Error RoomNameIsExist =>
        Error.NotFound("Room_Name_IsExist", "Room Name is Exist");
    public static Error NameRequired =>
        Error.Validation("Room_Name_Required", "Room name is required");

    public static Error CapacityInvalid =>
        Error.Validation("Room_Capacity_Invalid", "Room capacity must be greater than zero");

    public static Error InvalidSeatCount =>
        Error.Validation("Room_Seat_Count_Invalid", "Seat count must be greater than zero");

    public static Error AvailableSeatsInvalid =>
        Error.Validation("Room_Available_Seats_Invalid", "Available seats cannot be negative.");

    public static Error NotEnoughAvailableSeats(int availableSeats) =>
        Error.Validation("Room_Not_Enough_Available_Seats", $"Not enough available seats. Available: {availableSeats}.");

    public static Error AvailableSeatsOverflow =>
        Error.Unexpected("Room_Available_Seats_Overflow", "Available seats cannot exceed room capacity.");

    public static Error CapacityBelowReservedSeats(int reservedSeats) =>
        Error.Validation(
            "Room_Capacity_Below_Reserved_Seats",
            $"Room capacity cannot be less than the currently reserved seats: {reservedSeats}.");

    public static Error AvailableSeatsInconsistent(int expectedAvailableSeats) =>
        Error.Validation(
            "Room_Available_Seats_Inconsistent",
            $"Available seats must be {expectedAvailableSeats} to stay consistent with existing bookings.");
}
