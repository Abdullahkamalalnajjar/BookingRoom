using System.Linq;
using System.Security.Claims;

using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Application.Common.Security;
using BookingRoom.Application.Features.Identity.Commands.RegisterUser;
using BookingRoom.Application.Features.Identity.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookingRoom.Domain.Common.Results;
using BookingRoom.Domain.Identity;

namespace BookingRoom.Infrastructure.Identity;

public class IdentityService(
    IAppDbContext context,
    UserManager<AppUser> userManager,
    IUserClaimsPrincipalFactory<AppUser> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService,
    RoleManager<IdentityRole> roleManager,
    IdentityClaimsFactory identityClaimsFactory) : IIdentityService
{
    private readonly IAppDbContext _context = context;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly IUserClaimsPrincipalFactory<AppUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IdentityClaimsFactory _identityClaimsFactory = identityClaimsFactory;

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await FindActiveUserByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string? policyName)
    {
        if (string.IsNullOrWhiteSpace(policyName))
        {
            return false;
        }

        var user = await FindActiveUserByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var storedClaims = await _userManager.GetClaimsAsync(user);
        var effectiveClaims = _identityClaimsFactory.Create(roles, storedClaims);

        if (principal.Identity is ClaimsIdentity identity)
        {
            _identityClaimsFactory.AddMissingClaims(identity, effectiveClaims);
        }

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result<AppUserDto>> AuthenticateAsync(string email, string password)
    {
        var user = await FindActiveUserByEmailAsync(email);

        if (user is null)
        {
            return Error.Unauthorized("Invalid_Login_Attempt", "Email / Password are incorrect");
        }

        if (!user.EmailConfirmed)
        {
            return Error.Conflict("Email_Not_Confirmed", $"email '{UtilityService.MaskEmail(email)}' not confirmed");
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return Error.Unauthorized("Invalid_Login_Attempt", "Email / Password are incorrect");
        }

        return await BuildAppUserDtoAsync(user);
    }

    public async Task<Result<AppUserDto>> GetUserByIdAsync(string userId)
    {
        var user = await FindActiveUserByIdAsync(userId);

        if (user is null)
        {
            return Error.NotFound("User_Not_Found", $"User with id '{userId}' was not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        return await BuildAppUserDtoAsync(user, roles);
    }

    public Task<List<AppUserDto>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        return GetUsersByDeletionStatusAsync(isDeleted: false, cancellationToken);
    }

    public Task<List<AppUserDto>> GetDeletedUsersAsync(CancellationToken cancellationToken = default)
    {
        return GetUsersByDeletionStatusAsync(isDeleted: true, cancellationToken);
    }

    public async Task<Result<Deleted>> SoftDeleteAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null || user.IsDeleted)
        {
            return Error.NotFound("User_Not_Found", $"User with id '{userId}' was not found.");
        }

        user.IsDeleted = true;
        user.DeletedAtUtc = DateTimeOffset.UtcNow;
        user.LockoutEnabled = true;
        user.LockoutEnd = DateTimeOffset.MaxValue;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return ToErrors(updateResult.Errors);
        }

        await _userManager.UpdateSecurityStampAsync(user);

        await _context.RefreshTokens
            .Where(refreshToken => refreshToken.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Deleted;
    }

    public async Task<Result<Updated>> RestoreDeletedUserAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = _userManager.NormalizeEmail(email);
        var user = await _userManager.Users
            .FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);

        if (user is null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return Error.Unauthorized("Invalid_Login_Attempt", "Email / Password are incorrect");
        }

        if (!user.IsDeleted)
        {
            return Error.Conflict(
                "User_Already_Active",
                $"User with email '{UtilityService.MaskEmail(email)}' is already active.");
        }

        user.IsDeleted = false;
        user.DeletedAtUtc = null;
        user.LockoutEnd = null;
        user.AccessFailedCount = 0;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return ToErrors(updateResult.Errors);
        }

        await _userManager.UpdateSecurityStampAsync(user);

        return Result.Updated;
    }

    public async Task<Result<AppUserDto>> RegisterAsync(RegisterUserCommand request, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<Role>(request.Role, true, out var parsedRole))
        {
            return Error.Validation("Role_Invalid", $"Role must be one of: {string.Join(", ", Enum.GetNames<Role>())}.");
        }

        if (parsedRole == Role.Member)
        {
            if (string.IsNullOrWhiteSpace(request.City))
            {
                return Error.Validation("City_Required", "City is required for members.");
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return Error.Validation("Phone_Required", "Phone number is required for members.");
            }
        }

        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return Error.Conflict("Email_Already_Exists", $"A user with email {UtilityService.MaskEmail(request.Email)} already exists.");
        }

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber,
            City = parsedRole == Role.Member ? request.City?.Trim() : null,
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
        {
            return ToErrors(createResult.Errors);
        }

        var roleName = parsedRole.ToString();
        var ensureRoleResult = await EnsureRoleExistsAsync(roleName);

        if (ensureRoleResult.IsError)
        {
            await _userManager.DeleteAsync(user);
            return ensureRoleResult.Errors;
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, roleName);

        if (!addToRoleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return ToErrors(addToRoleResult.Errors);
        }

        return await BuildAppUserDtoAsync(user);
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await FindActiveUserByIdAsync(userId);

        return user?.UserName;
    }

    public async Task<Dictionary<string, string?>> GetUserNamesAsync(List<string> userIds)
    {
        ArgumentNullException.ThrowIfNull(userIds);

        var distinctIds = userIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToList();

        if (distinctIds.Count == 0)
        {
            return new Dictionary<string, string?>();
        }

        var users = await _userManager.Users
            .Where(user => distinctIds.Contains(user.Id) && !user.IsDeleted)
            .Select(user => new { user.Id, user.UserName })
            .ToListAsync();

        return users.ToDictionary(user => user.Id, user => user.UserName);
    }

    private async Task<Result<Success>> EnsureRoleExistsAsync(string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var createRoleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (!createRoleResult.Succeeded)
            {
                return ToErrors(createRoleResult.Errors);
            }
        }

        return Result.Success;
    }

    private static List<Error> ToErrors(IEnumerable<IdentityError> identityErrors)
    {
        return identityErrors
            .Select(error => Error.Validation(
                string.IsNullOrWhiteSpace(error.Code) ? "Identity_Error" : error.Code,
                string.IsNullOrWhiteSpace(error.Description) ? "Identity operation failed." : error.Description))
            .ToList();
    }

    private async Task<AppUserDto> BuildAppUserDtoAsync(AppUser user, IList<string>? roles = null)
    {
        roles ??= await _userManager.GetRolesAsync(user);

        var storedClaims = await _userManager.GetClaimsAsync(user);
        var effectiveClaims = _identityClaimsFactory.Create(roles, storedClaims);

        return new AppUserDto(
            user.Id,
            user.Email!,
            roles,
            effectiveClaims,
            user.City,
            user.PhoneNumber);
    }

    private Task<AppUser?> FindActiveUserByIdAsync(string userId)
    {
        return _userManager.Users.FirstOrDefaultAsync(user => user.Id == userId && !user.IsDeleted);
    }

    private Task<AppUser?> FindActiveUserByEmailAsync(string email)
    {
        var normalizedEmail = _userManager.NormalizeEmail(email);
        return _userManager.Users.FirstOrDefaultAsync(
            user => user.NormalizedEmail == normalizedEmail && !user.IsDeleted);
    }

    private async Task<List<AppUserDto>> GetUsersByDeletionStatusAsync(bool isDeleted, CancellationToken cancellationToken)
    {
        var users = await _userManager.Users
            .AsNoTracking()
            .Where(user => user.IsDeleted == isDeleted)
            .OrderBy(user => user.Email)
            .ToListAsync(cancellationToken);

        var result = new List<AppUserDto>(users.Count);

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(await BuildAppUserDtoAsync(user, roles));
        }

        return result;
    }
}
