using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Application.Features.Bookings.Dtos;
using BookingRoom.Domain.Common.Results;

namespace BookingRoom.Application.Features.Bookings.Queries.GetBookingsForMember;

public sealed record GetBookingsForMemberQuery() : ICachedQuery<Result<List<BookingDto>>>
{
    private static readonly string[] s_tags = new[] { "booking" };

    public string CacheKey => "bookingsForMember";

    public string[] Tags => s_tags;

    public TimeSpan Expiration => TimeSpan.FromMinutes(1);
}