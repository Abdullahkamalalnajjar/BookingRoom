using Microsoft.AspNetCore.Identity;

namespace BookingRoom.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public string? City { get; set; }
}
