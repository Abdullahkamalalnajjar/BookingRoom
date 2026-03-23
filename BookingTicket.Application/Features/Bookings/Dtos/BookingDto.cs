namespace BookingTicket.Application.Features.Bookings.Dtos;

public class BookingDto
{
    public Guid Id { get; set; }
    public int seats { get; set; }
    public string Status { get; set; }
}