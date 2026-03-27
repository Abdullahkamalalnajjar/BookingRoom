using BookingRoom.Domain.Common.Results;

using MediatR;

namespace BookingRoom.Application.Features.Identity.Queries.RefreshTokens;

public record RefreshTokenQuery(string RefreshToken, string ExpiredAccessToken) : IRequest<Result<TokenResponse>>;