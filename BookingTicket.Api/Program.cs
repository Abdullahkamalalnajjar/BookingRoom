using BookingTicket.Infrastructure;
using BookingTicket.Application.Common.Behaviours;
using BookingTicket.Application.Features.Bookings.Commands.CreateBooking;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<CreateBookingCommand>();
});

// Validators (keep it explicit to avoid adding extra FluentValidation DI packages).
builder.Services.AddScoped<IValidator<CreateBookingCommand>, CreateBookingCommandValidator>();

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

app.MapControllers();

app.Run();
