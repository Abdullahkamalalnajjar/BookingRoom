using System.Security.Claims;

//using DDD.Application.Features.Identity;
//using DDD.Application.Features.Identity.Dtos;

namespace BookingRoom.Application.Common.Interfaces;

public interface ITokenProvider
{
    //    Task<Result<TokenResponse>> GenerateJwtTokenAsync(AppUserDto user, CancellationToken ct = default);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
