using BookingRoom.Domain.Bookings.Events;
using BookingRoom.Domain.Common;
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
    public BookingStatus Status { get; private set; } = BookingStatus.Booking;
    

    private Booking()
    {
    }

    private Booking(Guid id, string userId, Guid roomId, int seats) : base(id)
    {
        UserId = userId;
        RoomId = roomId;
        Seats = seats;
    }

    public static Result<Booking> Create(Guid id, string userId, Guid roomId, int seats)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BookingErrors.UserRequired;

        if (roomId == Guid.Empty)
            return BookingErrors.RoomRequired;

        if (seats <= 0)
            return BookingErrors.SeatInvalid;

        var booking = new Booking(id, userId, roomId, seats);
        booking.AddDomainEvent(new BookingChangedEvent(booking, BookingChangeType.Created));
        return booking;
    }

    public Result<Updated> Update(int seats, BookingStatus status)
    {
        if (seats <= 0)
            return BookingErrors.SeatInvalid;
        Seats = seats;
        Status = status;
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
    
}
