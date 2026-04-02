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

    Task<List<AppUserDto>> GetActiveUsersAsync(CancellationToken cancellationToken = default);

    Task<List<AppUserDto>> GetDeletedUsersAsync(CancellationToken cancellationToken = default);

    Task<Result<Deleted>> SoftDeleteAsync(string userId, CancellationToken cancellationToken = default);

    Task<Result<Updated>> RestoreDeletedUserAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<string?> GetUserNameAsync(string userId);

    Task<Dictionary<string, string?>> GetUserNamesAsync(List<string> userIds);
}
