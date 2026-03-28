namespace BookingRoom.Application.Features.Bookings.Dtos;

public class BookingDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public Guid RoomId { get; set; }
    public required string RoomName {get; set;}
    public int Seats { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
