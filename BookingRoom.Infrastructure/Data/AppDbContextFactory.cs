using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using BookingRoom.Infrastructure.Common;
 
namespace BookingRoom.Infrastructure.Data;

/// <summary>
/// Enables `dotnet ef` to create AppDbContext at design-time (migrations) without relying on runtime DI.
/// </summary>
public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Missing connection string 'DefaultConnection'. " +
                "Ensure it exists in appsettings.json (e.g. BookingRoom.Api/appsettings.json) or as an environment variable.");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString);

        return new AppDbContext(optionsBuilder.Options, new NullMediator());
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var currentDir = Directory.GetCurrentDirectory();

        var builder = new ConfigurationBuilder()
            .SetBasePath(currentDir)
            .AddEnvironmentVariables();

        // Prefer the API project's appsettings if present (common setup in layered solutions).
        AddIfFound(builder, currentDir, "BookingRoom.Api/appsettings.json");
        AddIfFound(builder, currentDir, $"BookingRoom.Api/appsettings.{env}.json");

        // Fallback to local appsettings if the command is executed from the API directory.
        builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        builder.AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false);

        return builder.Build();
    }

    private static void AddIfFound(IConfigurationBuilder builder, string startDir, string relativePath)
    {
        var fullPath = FindFileUpwards(startDir, relativePath);
        if (fullPath is not null)
            builder.AddJsonFile(fullPath, optional: true, reloadOnChange: false);
    }

    private static string? FindFileUpwards(string startDir, string relativePath)
    {
        var dir = new DirectoryInfo(startDir);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, relativePath);
            if (File.Exists(candidate))
                return candidate;
            dir = dir.Parent;
        }

        return null;
    }
}
