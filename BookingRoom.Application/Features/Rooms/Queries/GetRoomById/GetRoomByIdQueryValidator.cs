using FluentValidation;

namespace BookingRoom.Application.Features.Rooms.Queries.GetRoomById;

public class GetRoomByIdQueryValidator : AbstractValidator<GetRoomByIdQuery>
{
    public GetRoomByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithErrorCode("Room_Id_Required").WithMessage("Id cannot be empty");
    }
}