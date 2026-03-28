using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Application.Features.Identity.Dtos;
using BookingRoom.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace BookingRoom.Application.Features.Identity.Queries.GetUserInfo;

public class GetUserByIdQueryHanlder(ILogger<GetUserByIdQueryHanlder> logger, IIdentityService identityService)
    : IRequestHandler<GetUserByIdQuery, Result<AppUserDto>>
{
    private readonly ILogger<GetUserByIdQueryHanlder> _logger = logger;
    private readonly IIdentityService _identityService = identityService;

    public async Task<Result<AppUserDto>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var getUserByIdResult = await _identityService.GetUserByIdAsync(request.UserId!);

        if (!getUserByIdResult.IsError) return getUserByIdResult.Value;
        _logger.LogError("User with Id { UserId }{ErrorDetails}", request.UserId, getUserByIdResult.TopError.Description);

        return getUserByIdResult.Errors;

    }
}