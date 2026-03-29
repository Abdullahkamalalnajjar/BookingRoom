using BookingRoom.Domain.Bookings;
using BookingRoom.Domain.Common;
using BookingRoom.Domain.Common.Results;

namespace BookingRoom.Domain.Rooms;

public class Room : AuditableEntity
{
    private readonly List<Booking> _bookings = [];

    public string Name { get; private set; } = string.Empty;
    public int SeatCapacity { get; private set; }
    public int AvailableSeats { get; private set; }
    public decimal SeatPrice { get; private set; }
    public IReadOnlyCollection<Booking> Bookings => _bookings.AsReadOnly();

    private Room()
    {
    }

    private Room(Guid id, string name, int seatCapacity, decimal seatPrice) : base(id)
    {
        Name = name;
        SeatCapacity = seatCapacity;
        AvailableSeats = seatCapacity;
        SeatPrice = seatPrice;
    }

    public static Result<Room> Create(Guid id, string name, int seatCapacity, decimal seatPrice)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return RoomErrors.NameRequired;
        }

        if (seatCapacity <= 0)
        {
            return RoomErrors.CapacityInvalid;
        }

        if (seatPrice < 0)
        {
            return RoomErrors.SeatPriceInvalid;
        }

        return new Room(id, name.Trim(), seatCapacity, seatPrice);
    }

    public Result<Updated> Update(string name, int seatCapacity, int availableSeats, decimal seatPrice)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return RoomErrors.NameRequired;
        }

        if (seatCapacity <= 0)
        {
            return RoomErrors.CapacityInvalid;
        }

        if (availableSeats < 0)
        {
            return RoomErrors.AvailableSeatsInvalid;
        }

        if (availableSeats > seatCapacity)
        {
            return RoomErrors.AvailableSeatsOverflow;
        }

        if (seatPrice < 0)
        {
            return RoomErrors.SeatPriceInvalid;
        }

        var reservedSeats = SeatCapacity - AvailableSeats;

        if (seatCapacity < reservedSeats)
        {
            return RoomErrors.CapacityBelowReservedSeats(reservedSeats);
        }

        var expectedAvailableSeats = seatCapacity - reservedSeats;
        if (availableSeats != expectedAvailableSeats)
        {
            return RoomErrors.AvailableSeatsInconsistent(expectedAvailableSeats);
        }

        Name = name.Trim();
        SeatCapacity = seatCapacity;
        AvailableSeats = availableSeats;
        SeatPrice = seatPrice;

        return Result.Updated;
    }

    public Result<Updated> ReserveSeats(int seats) // reduce available seats
    {
        if (seats <= 0)
        {
            return RoomErrors.InvalidSeatCount;
        }

        if (seats > AvailableSeats)
        {
            return RoomErrors.NotEnoughAvailableSeats(AvailableSeats);
        }

        AvailableSeats -= seats;
        return Result.Updated;
    }

    public Result<Updated> ReleaseSeats(int seats) // increase available
    {
        if (seats <= 0)
        {
            return RoomErrors.InvalidSeatCount;
        }

        if (AvailableSeats + seats > SeatCapacity)
        {
            return RoomErrors.AvailableSeatsOverflow;
        }

        AvailableSeats += seats;
        return Result.Updated;
    }
}
