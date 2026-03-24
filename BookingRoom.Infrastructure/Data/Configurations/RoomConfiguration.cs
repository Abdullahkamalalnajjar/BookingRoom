using BookingRoom.Domain.Rooms;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingRoom.Infrastructure.Data.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(r => r.Id).IsClustered(false);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
        builder.Property(r => r.SeatCapacity).IsRequired();
        builder.Property(r => r.AvailableSeats).IsRequired();
    }
}
