using System.Text;

using BookingRoom.Api;
using BookingRoom.Api.Infrastructure;
using BookingRoom.Application;
using BookingRoom.Infrastructure;
using BookingRoom.Infrastructure.Data;
using BookingRoom.Infrastructure.Data.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddHybridCache();

    builder.Services.AddApiServices(builder.Configuration);
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    var app = builder.Build();

    var applyMigrationsOnStartup =
        builder.Configuration.GetValue<bool?>("Database:ApplyMigrationsOnStartup")
        ?? app.Environment.IsDevelopment();

    if (applyMigrationsOnStartup)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("Startup");

        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync();

            var seeder = scope.ServiceProvider.GetRequiredService<AppDbContextSeeder>();
            await seeder.SeedAsync();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Application startup database initialization failed.");

            if (app.Environment.IsDevelopment())
            {
                throw;
            }
        }
    }

    // Configure the HTTP request pipeline.
   // if (app.Environment.IsDevelopment())
  //  {
        app.UseSwagger();
        app.UseSwaggerUI();
  //  }

    app.UseMiddleware<RequestLogContextMiddleware>();
    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseCors("ApiCors");
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseOutputCache();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    await WriteStartupFailureAsync(exception);
    throw;
}

static async Task WriteStartupFailureAsync(Exception exception)
{
    try
    {
        var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(logDirectory);

        var logPath = Path.Combine(logDirectory, "startup-error.log");
        var content = new StringBuilder()
            .AppendLine($"UTC: {DateTime.UtcNow:O}")
            .AppendLine(exception.ToString())
            .AppendLine(new string('-', 80))
            .ToString();

        await File.AppendAllTextAsync(logPath, content);
    }
    catch
    {
        // Avoid masking the original startup failure if writing the diagnostics log also fails.
    }
}
