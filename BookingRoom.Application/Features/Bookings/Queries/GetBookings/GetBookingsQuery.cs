using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Application.Features.Bookings.Dtos;
using BookingRoom.Domain.Common.Results;
using System;

namespace BookingRoom.Application.Features.Bookings.Queries.GetBookings;

public sealed record GetBookingsQuery() : ICachedQuery<Result<List<BookingDto>>>
{
    private static readonly string[] s_tags = new[] { "booking" };

    public string CacheKey => "bookings";

    public string[] Tags => s_tags;

    public TimeSpan Expiration => TimeSpan.FromMinutes(1);
}
