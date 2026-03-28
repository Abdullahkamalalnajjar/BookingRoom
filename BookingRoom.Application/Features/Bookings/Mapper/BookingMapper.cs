using System.Linq;

using BookingRoom.Application.Features.Bookings.Dtos;
using BookingRoom.Domain.Bookings;

namespace BookingRoom.Application.Features.Bookings.Mapper;

public static class BookingMapper
{
    public static BookingDto ToDo(this Booking booking, string? roomName = null, string? userName = null)
    {
        ArgumentNullException.ThrowIfNull(booking);

        return new BookingDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            Username = userName ?? string.Empty,
            RoomId = booking.RoomId,
            RoomName = roomName ?? booking.Room?.Name ?? string.Empty,
            Seats = booking.Seats,
            Status = booking.Status.ToString(),
            CreatedAtUtc = booking.CreatedAtUtc.UtcDateTime,
            UpdatedAtUtc = booking.LastModifiedUtc.UtcDateTime,
        };
    }

    public static async Task<List<BookingDto>> ToDtosAsync(
        this IEnumerable<Booking> bookings,
        Func<List<string>, Task<Dictionary<string, string?>>> resolveUserNamesAsync)
    {
        ArgumentNullException.ThrowIfNull(bookings);
        ArgumentNullException.ThrowIfNull(resolveUserNamesAsync);

        var bookingList = bookings.ToList();

        var userIds = bookingList
            .Select(b => b.UserId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToList();

        var userNames = await resolveUserNamesAsync(userIds);

        return bookingList
            .Select(booking =>
            {
                userNames.TryGetValue(booking.UserId, out var userName);
                return booking.ToDo(userName: userName);
            })
            .ToList();
    }
}
