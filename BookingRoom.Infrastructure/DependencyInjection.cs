using BookingRoom.Application.Common.Interfaces;
using BookingRoom.Application.Common.Security;
using BookingRoom.Infrastructure.Bookings;
using BookingRoom.Infrastructure.Data;
using BookingRoom.Infrastructure.Data.Interceptors;
using BookingRoom.Infrastructure.Data.Seed;
using BookingRoom.Infrastructure.Identity;
using BookingRoom.Infrastructure.Notifications;
using BookingRoom.Infrastructure.Payments;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace BookingRoom.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<IdentityClaimsFactory>();
        services.AddScoped<AppDbContextSeeder>();
        services.Configure<SeedDataOptions>(configuration.GetSection("SeedData"));
        services.Configure<PaymobOptions>(configuration.GetSection(PaymobOptions.SectionName));
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
        services.Configure<PendingBookingExpirationOptions>(
            configuration.GetSection(PendingBookingExpirationOptions.SectionName));

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            options
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(serviceProvider.GetRequiredService<AuditableEntityInterceptor>()));

        services
            .AddIdentityCore<AppUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        var jwtSettings = configuration.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JwtSettings:Secret is missing.");
        var issuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JwtSettings:Issuer is missing.");
        var audience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JwtSettings:Audience is missing.");
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                        if (string.IsNullOrWhiteSpace(userId))
                        {
                            context.Fail("The access token does not contain a valid user identifier.");
                            return;
                        }

                        var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
                        var user = await userManager.Users
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.Id == userId);

                        if (user is null || user.IsDeleted)
                        {
                            context.Fail("The user account is no longer active.");
                        }
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.AuthenticatedUser, policy =>
                policy.RequireAuthenticatedUser());

            options.AddPolicy(AuthorizationPolicies.RoomsRead, policy =>
                policy.RequireAuthenticatedUser()
                    .RequireClaim(PermissionClaimTypes.Permission, Permissions.Rooms.Read));

            options.AddPolicy(AuthorizationPolicies.RoomsWrite, policy =>
                policy.RequireAuthenticatedUser()
                    .RequireClaim(PermissionClaimTypes.Permission, Permissions.Rooms.Write));

            options.AddPolicy(AuthorizationPolicies.BookingsRead, policy =>
                policy.RequireAuthenticatedUser()
                    .RequireClaim(PermissionClaimTypes.Permission, Permissions.Bookings.Read));

            options.AddPolicy(AuthorizationPolicies.BookingsWrite, policy =>
                policy.RequireAuthenticatedUser()
                    .RequireClaim(PermissionClaimTypes.Permission, Permissions.Bookings.Write));

            options.AddPolicy(AuthorizationPolicies.CurrentUserRead, policy =>
                policy.RequireAuthenticatedUser()
                    .RequireClaim(PermissionClaimTypes.Permission, Permissions.Users.ReadSelf));

            options.AddPolicy(AuthorizationPolicies.UsersWrite, policy =>
                policy.RequireAuthenticatedUser()
                    .RequireClaim(PermissionClaimTypes.Permission, Permissions.Users.Write));
        });

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<INotificationService, SmtpNotificationService>();
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<IPaymobWebhookService, PaymobWebhookService>();
        services.AddHttpClient<IPaymentCheckoutService, PaymobCheckoutService>();
        services.AddHostedService<PendingBookingExpirationService>();

        return services;
    }
}
