using System.Linq;

using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Domain.Bookings;
using BookingRoom.Domain.Common;
using BookingRoom.Domain.Identity;
using BookingRoom.Domain.Rooms;
using BookingRoom.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookingRoom.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator)
    : IdentityDbContext<AppUser, IdentityRole, string>(options), IAppDbContext
{
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply Configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        await DispatchDomainEventsAsync(cancellationToken);
        return result;
    }

    public override int SaveChanges()
    {
        var result = base.SaveChanges();
        DispatchDomainEventsAsync(CancellationToken.None).GetAwaiter().GetResult();
        return result;
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEntities = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .Where(entity => entity.DomainEvents.Any())
            .ToList();

        if (!domainEntities.Any())
        {
            return;
        }

        var domainEvents = domainEntities
            .SelectMany(entity => entity.DomainEvents)
            .ToList();

        foreach (var entity in domainEntities)
        {
            entity.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            await mediator.Publish(domainEvent, cancellationToken);
        }
    }
}
