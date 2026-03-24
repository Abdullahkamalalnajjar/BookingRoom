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

    public IReadOnlyCollection<Booking> Bookings => _bookings.AsReadOnly();

    private Room()
    {
    }

    private Room(Guid id, string name, int seatCapacity) : base(id)
    {
        Name = name;
        SeatCapacity = seatCapacity;
        AvailableSeats = seatCapacity;
    }

    public static Result<Room> Create(Guid id, string name, int seatCapacity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return RoomErrors.NameRequired;
        }

        if (seatCapacity <= 0)
        {
            return RoomErrors.CapacityInvalid;
        }

        return new Room(id, name.Trim(), seatCapacity);
    }

    public Result<Updated> ReserveSeats(int seats)
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

    public Result<Updated> ReleaseSeats(int seats)
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
