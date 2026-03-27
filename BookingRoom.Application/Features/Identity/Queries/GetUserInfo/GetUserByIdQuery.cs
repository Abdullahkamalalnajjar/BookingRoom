using BookingRoom.Application.Features.Identity.Dtos;
using BookingRoom.Domain.Common.Results;

using MediatR;

namespace BookingRoom.Application.Features.Identity.Queries.GetUserInfo;

public sealed record GetUserByIdQuery(string? UserId) : IRequest<Result<AppUserDto>>;