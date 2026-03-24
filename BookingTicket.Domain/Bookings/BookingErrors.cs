using BookingTicket.Domain.Common.Results;

namespace BookingTicket.Domain.Bookings;

public static class BookingErrors
{
    public static Error BookingNotFound =>
    Error.NotFound("Booking_Not_Found","Booking Not found");
    
    public static Error SeatRequired =>
    Error.Validation("Seat_Required","Seat required");
    
    public static Error SeatInvalid =>
    Error.Validation("Seat_Invalid","Seat invalid");

    public static Error RoomRequired =>
    Error.Validation("Room_Required","Room is required");

    public static Error SeatExceedsRoomCapacity(int capacity) =>
    Error.Validation("Seat_Exceeds_Room_Capacity",$"Seats cannot exceed room capacity ({capacity}).");
}
