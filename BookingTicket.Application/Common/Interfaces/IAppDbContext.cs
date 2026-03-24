using BookingTicket.Domain.Bookings;
using BookingTicket.Domain.Rooms;
using Microsoft.EntityFrameworkCore;

namespace BookingTicket.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Booking> Bookings { get; }
    DbSet<Room> Rooms { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
