using BookingTicket.Application.Features.Bookings.Dtos;
using BookingTicket.Domain.Bookings;

namespace BookingTicket.Application.Features.Bookings.Mapper;

public static class BookingMapper
{
    public static BookingDto ToDo(this Booking booking)
    {
        ArgumentNullException.ThrowIfNull(booking);

        return new BookingDto
        {
            Id = booking.Id,
            seats = booking.Seats,
            Status = booking.Status.ToString(),
        };
    }

    public static List<BookingDto> ToDtos(this IEnumerable<Booking> bookings)
    {
        return bookings.Select(booking=> ToDo(booking)!).ToList();
    }
}