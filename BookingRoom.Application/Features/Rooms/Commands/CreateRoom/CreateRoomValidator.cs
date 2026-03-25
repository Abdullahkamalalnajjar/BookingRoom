using FluentValidation;

namespace BookingRoom.Application.Features.Rooms.Commands.CreateRoom;

public class CreateRoomValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.SeatCapacity).NotEmpty().WithMessage("SeatCapacity is required.");
    }
}