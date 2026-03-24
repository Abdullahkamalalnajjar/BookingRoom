using BookingTicket.Domain.Bookings;
using BookingTicket.Domain.Rooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingTicket.Infrastructure.Data.Configurations;

public class BookingConfiguration:IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id).IsClustered(false);
        builder.Property(s => s.Seats).IsRequired();
        builder.Property(s => s.RoomId).IsRequired();

        builder.HasOne<Room>(b => b.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
