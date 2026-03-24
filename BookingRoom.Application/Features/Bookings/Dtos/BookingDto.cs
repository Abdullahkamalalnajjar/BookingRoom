namespace BookingRoom.Application.Features.Bookings.Dtos;

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public required string RoomName {get; set;}
    public int seats { get; set; }
    public string Status { get; set; } = string.Empty;
}
