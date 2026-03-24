using FluentValidation;

namespace BookingRoom.Application.Features.Bookings.Commands.CreateBooking;

public sealed class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.RoomId)
            .NotEmpty()
            .WithMessage("RoomId is required.");

        RuleFor(x => x.Seats)
            .GreaterThan(0)
            .WithMessage("Seats must be greater than 0.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status is invalid.");
    }
}
