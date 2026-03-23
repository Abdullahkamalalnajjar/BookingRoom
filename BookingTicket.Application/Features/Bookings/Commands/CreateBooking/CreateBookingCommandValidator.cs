using FluentValidation;

namespace BookingTicket.Application.Features.Bookings.Commands.CreateBooking;

public sealed class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.seats)
            .GreaterThan(0)
            .WithMessage("Seats must be greater than 0.");

        RuleFor(x => x.status)
            .IsInEnum()
            .WithMessage("Status is invalid.");
    }
}
