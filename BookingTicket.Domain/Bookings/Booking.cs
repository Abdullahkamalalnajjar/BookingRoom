using BookingTicket.Domain.Common;
using BookingTicket.Domain.Common.Results;
using BookingTicket.Domain.Enums;

namespace BookingTicket.Domain.Bookings;

public class Booking : AuditableEntity
{
    public int Seats { get; private set; }
    public BookingStatus Status { get; private set; } = BookingStatus.Booking;

    private Booking()
    {
    }
    
    private  Booking(Guid id,int seats):base(id)
    {
        Seats = seats;
    }

    public static Result<Booking> Create(Guid id,int seats)
    {
        if (seats <= 0)
            return BookingErrors.SeatInvalid;
        return new Booking(id,seats);
    }

    public Result<Updated> Update(int seats,BookingStatus status)
    {
        if (seats <= 0)
            return BookingErrors.SeatInvalid;
        Seats = seats;
        Status = status;
        return Result.Updated;
    }
    
    
        

}