using BookingRoom.Domain.Bookings;
using BookingRoom.Domain.Rooms;
using BookingRoom.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingRoom.Infrastructure.Data.Configurations;

public class BookingConfiguration:IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id).IsClustered(false);
        builder.Property(s => s.Seats).IsRequired();
        builder.Property(s => s.UserId).IsRequired().HasMaxLength(450);
        builder.Property(s => s.RoomId).IsRequired();

        builder.HasIndex(b => b.UserId);

        builder.HasOne<Room>(b => b.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<AppUser>()
            .WithMany(user => user.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
