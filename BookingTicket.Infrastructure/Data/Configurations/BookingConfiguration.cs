using BookingTicket.Domain.Bookings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingTicket.Infrastructure.Data.Configurations;

public class BookingConfiguration:IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id).IsClustered(false);
        builder.Property(s => s.Seats).IsRequired();
        
    }
}