using BookingRoom.Infrastructure;
using BookingRoom.Application.Common.Behaviours;
using BookingRoom.Application.Features.Bookings.Commands.CreateBooking;
using BookingRoom.Application.Features.Bookings.Queries.GetBookingById;
using BookingRoom.Application.Features.Bookings.Queries.GetBookings;
using Asp.Versioning;
using BookingRoom.Application.Features.Bookings.Queries.GetBookingByStatus;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});
builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<CreateBookingCommand>();
});

// Validators (keep it explicit to avoid adding extra FluentValidation DI packages).
builder.Services.AddScoped<IValidator<CreateBookingCommand>, CreateBookingCommandValidator>();
builder.Services.AddScoped<IValidator<GetBookingQuery>, GetBookingByIdValidator>();
builder.Services.AddScoped<IValidator<GetBookingByStatusQuery>, GetBookingByStatusValidation>();

// MediatR pipeline
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddInfrastructure(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseOutputCache();

app.MapControllers();

app.Run();
