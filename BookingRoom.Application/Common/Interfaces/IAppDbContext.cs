using BookingRoom.Domain.Bookings;
using BookingRoom.Domain.Identity;
using BookingRoom.Domain.Rooms;
using Microsoft.EntityFrameworkCore;

namespace BookingRoom.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Booking> Bookings { get; }
    DbSet<Room> Rooms { get; }
    public DbSet<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
