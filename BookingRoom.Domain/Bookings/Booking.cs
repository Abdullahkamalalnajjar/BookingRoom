using BookingRoom.Domain.Bookings.Events;
using BookingRoom.Domain.Common;
using BookingRoom.Domain.Common.Costants;
using BookingRoom.Domain.Common.Results;
using BookingRoom.Domain.Enums;
using BookingRoom.Domain.Rooms;

namespace BookingRoom.Domain.Bookings;

public class Booking : AuditableEntity
{
    public string UserId { get; private set; } = string.Empty;
    public Guid RoomId { get; private set; }
    public Room? Room { get; private set; }
    public int Seats { get; private set; }
    public decimal SubPrice { get; private set; }
    public decimal TotalPrice { get; private set; }
    public BookingStatus Status { get; private set; } = BookingStatus.Booking;
    public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.Pending;
    

    private Booking()
    {
    }

    private Booking(Guid id, string userId, Guid roomId, int seats, decimal seatPrice) : base(id)
    {
        UserId = userId;
        RoomId = roomId;
        Seats = seats;
        UpdatePricing(seatPrice);
    }

    public static Result<Booking> Create(Guid id, string userId, Guid roomId, int seats, decimal seatPrice)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BookingErrors.UserRequired;

        if (roomId == Guid.Empty)
            return BookingErrors.RoomRequired;

        if (seats <= 0)
            return BookingErrors.SeatInvalid;

        if (seatPrice < 0)
            return BookingErrors.SeatPriceInvalid;

        var booking = new Booking(id, userId, roomId, seats, seatPrice);
        booking.AddDomainEvent(new BookingChangedEvent(booking, BookingChangeType.Created));
        return booking;
    }

    public Result<Updated> Update(int seats, BookingStatus status, decimal seatPrice)
    {
        if (seats <= 0)
            return BookingErrors.SeatInvalid;

        if (seatPrice < 0)
            return BookingErrors.SeatPriceInvalid;

        Seats = seats;
        Status = status;
        UpdatePricing(seatPrice);
        AddDomainEvent(new BookingChangedEvent(this, BookingChangeType.Updated));
        return Result.Updated;
    }

    public Result<Updated> ChangeRoom(Guid roomId)
    {
        if (roomId == Guid.Empty)
            return BookingErrors.RoomRequired;

        RoomId = roomId;
        return Result.Updated;
    }

    public Result<Updated> ChangeStatus(BookingStatus status)
    {
        Status = status;
        AddDomainEvent(new BookingChangedEvent(this, BookingChangeType.Updated));
        return Result.Updated;
    }

    public Result<Updated> ChangePaymentStatus(PaymentStatus paymentStatus)
    {
        PaymentStatus = paymentStatus;
        AddDomainEvent(new BookingChangedEvent(this, BookingChangeType.Updated));
        return Result.Updated;
    }

    private void UpdatePricing(decimal seatPrice)
    {
        SubPrice = seatPrice * Seats;
        TotalPrice = SubPrice + BookingRoomConstants.TaxRate;
    }
}
