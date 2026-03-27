using BookingRoom.Application.Features.Identity.Commands.RegisterUser;
using BookingRoom.Application.Features.Identity.Dtos;
using BookingRoom.Domain.Common.Results;

namespace BookingRoom.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string? policyName);

    Task<Result<AppUserDto>> RegisterAsync(RegisterUserCommand request, CancellationToken cancellationToken = default);

    Task<Result<AppUserDto>> AuthenticateAsync(string email, string password);

    Task<Result<AppUserDto>> GetUserByIdAsync(string userId);

    Task<string?> GetUserNameAsync(string userId);
}
