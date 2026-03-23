using BookingTicket.Domain.Bookings;
using Microsoft.EntityFrameworkCore;

namespace BookingTicket.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Booking> Bookings { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
