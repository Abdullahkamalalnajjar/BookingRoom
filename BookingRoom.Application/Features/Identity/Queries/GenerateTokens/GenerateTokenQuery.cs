using BookingRoom.Domain.Common.Results;

using MediatR;

namespace BookingRoom.Application.Features.Identity.Queries.GenerateTokens;

public record GenerateTokenQuery(
    string Email,
    string Password) : IRequest<Result<TokenResponse>>;