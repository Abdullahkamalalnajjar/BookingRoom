using BookingRoom.Domain.Common.Results;

namespace BookingRoom.Domain.Bookings;

public static class BookingErrors
{
    public static Error BookingNotFound =>
    Error.NotFound("Booking_Not_Found","Booking Not found");

    public static Error UserRequired =>
    Error.Validation("User_Required","User is required");
    
    public static Error SeatRequired =>
    Error.Validation("Seat_Required","Seat required");
    
    public static Error SeatInvalid =>
    Error.Validation("Seat_Invalid","Seat invalid");

    public static Error RoomRequired =>
    Error.Validation("Room_Required","Room is required");

    public static Error SeatExceedsRoomCapacity(int capacity) =>
    Error.Validation("Seat_Exceeds_Room_Capacity",$"Seats cannot exceed room capacity ({capacity}).");
}
