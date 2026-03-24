using FluentValidation;

namespace BookingTicket.Application.Features.Bookings.Queries.GetBookingById;

public class GetBookingByIdValidator : AbstractValidator<GetBookingQuery>
{
    public GetBookingByIdValidator()
    {
        RuleFor(x=>x.id).NotEmpty().WithErrorCode("Booking_Id_Required").WithMessage("Id cannot be empty");
    }
}