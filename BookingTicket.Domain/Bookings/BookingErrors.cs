using BookingTicket.Domain.Common.Results;

namespace BookingTicket.Domain.Bookings;

public static class BookingErrors
{
    public static Error BookingNotFound =>
    Error.NotFound("Booking_Not_Found","Booking not found");
    
    public static Error SeatRequired =>
    Error.Validation("Seat_Required","Seat required");
    
    public static Error SeatInvalid =>
    Error.Validation("Seat_Invalid","Seat invalid");
}
