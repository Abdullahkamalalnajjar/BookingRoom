using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Application.Features.Rooms.Dtos;
using BookingRoom.Domain.Common.Results;
using System;

namespace BookingRoom.Application.Features.Rooms.Queries.GetRooms;

public sealed record GetRoomsQuery() : ICachedQuery<Result<List<RoomDto>>>
{
    private static readonly string[] s_tags = new[] { "room" };

    public string CacheKey => "rooms";

    public string[] Tags => s_tags;

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
