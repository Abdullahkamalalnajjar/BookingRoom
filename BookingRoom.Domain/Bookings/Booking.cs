using BookingRoom.Domain.Common;
using BookingRoom.Domain.Common.Results;
using BookingRoom.Domain.Enums;
using BookingRoom.Domain.Rooms;

namespace BookingRoom.Domain.Bookings;

public class Booking : AuditableEntity
{
    public Guid RoomId { get; private set; }
    public Room? Room { get; private set; }
    public int Seats { get; private set; }
    public BookingStatus Status { get; private set; } = BookingStatus.Booking;

    private Booking()
    {
    }
    
    private  Booking(Guid id, Guid roomId, int seats):base(id)
    {
        RoomId = roomId;
        Seats = seats;
    }

    public static Result<Booking> Create(Guid id, Guid roomId, int seats)
    {
        if (roomId == Guid.Empty)
            return BookingErrors.RoomRequired;

        if (seats <= 0)
            return BookingErrors.SeatInvalid;

        return new Booking(id, roomId, seats);
    }

    public Result<Updated> Update(int seats,BookingStatus status)
    {
        if (seats <= 0)
            return BookingErrors.SeatInvalid;
        Seats = seats;
        Status = status;
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
        status = status;
        return Result.Updated;
    }
    
}
