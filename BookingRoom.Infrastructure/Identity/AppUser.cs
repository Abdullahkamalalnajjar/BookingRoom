using BookingRoom.Domain.Bookings;
using Microsoft.AspNetCore.Identity;

namespace BookingRoom.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public string? City { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAtUtc { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
