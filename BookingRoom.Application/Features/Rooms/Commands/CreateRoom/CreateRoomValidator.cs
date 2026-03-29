using FluentValidation;

namespace BookingRoom.Application.Features.Rooms.Commands.CreateRoom;

public class CreateRoomValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.SeatCapacity).GreaterThan(0).WithMessage("SeatCapacity must be greater than 0.");
        RuleFor(x => x.SeatPrice).GreaterThanOrEqualTo(0).WithMessage("SeatPrice cannot be negative.");
    }
}
