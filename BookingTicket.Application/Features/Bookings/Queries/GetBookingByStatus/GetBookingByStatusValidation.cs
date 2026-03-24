using BookingTicket.Domain.Enums;
using FluentValidation;

namespace BookingTicket.Application.Features.Bookings.Queries.GetBookingByStatus;

public class GetBookingByStatusValidation : AbstractValidator<GetBookingByStatusQuery>
{
    public GetBookingByStatusValidation()
    {
        RuleFor(s => s.status)
            .Must(BeValidStatus)
            .When(s => !string.IsNullOrWhiteSpace(s.status))
            .WithErrorCode("Booking_Status_Invalid")
            .WithMessage("الحالة المرسلة غير موجودة.");
    }

    private static bool BeValidStatus(string? status)
    {
        if (!Enum.TryParse<BookingStatus>(status, ignoreCase: true, out var parsed))
        {
            return false;
        }

        return Enum.IsDefined(typeof(BookingStatus), parsed);
    }
}
