using FluentValidation;

namespace BookingRoom.Application.Features.Rooms.Commands.UpdateRoom;

public class UpdateRoomValidator : AbstractValidator<UpdateRoomCommand>
{
    public UpdateRoomValidator()
    {
        RuleFor(x => x.RoomId)
            .NotEmpty()
            .WithErrorCode("Room_Id_Required")
            .WithMessage("RoomId cannot be empty.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.SeatCapacity)
            .GreaterThan(0)
            .WithMessage("SeatCapacity must be greater than 0.");

        RuleFor(x => x.AvailableSeats)
            .GreaterThanOrEqualTo(0)
            .WithMessage("AvailableSeats cannot be negative.");

        RuleFor(x => x.AvailableSeats)
            .LessThanOrEqualTo(x => x.SeatCapacity)
            .WithMessage("AvailableSeats cannot be greater than SeatCapacity.");
    }
}
