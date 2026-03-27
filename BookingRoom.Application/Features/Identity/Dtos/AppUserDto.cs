using System.Security.Claims;

namespace BookingRoom.Application.Features.Identity.Dtos;

public sealed record AppUserDto(
    string UserId,
    string Email,
    IList<string> Roles,
    IList<Claim> Claims,
    string? City = null,
    string? PhoneNumber = null);
