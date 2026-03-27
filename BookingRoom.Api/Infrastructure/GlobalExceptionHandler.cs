using BookingRoom.Api.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Hosting;

namespace BookingRoom.Api.Infrastructure;

public class GlobalExceptionHandler(IHostEnvironment hostEnvironment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        await httpContext
            .ToProblem(exception, hostEnvironment.IsDevelopment())
            .ExecuteAsync(httpContext);

        return true;
    }
}
