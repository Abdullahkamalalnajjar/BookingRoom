using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingRoom.Infrastructure.Migrations
{
    public partial class AddRoomAvailableSeats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableSeats",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE [Rooms] SET [AvailableSeats] = [SeatCapacity]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableSeats",
                table: "Rooms");
        }
    }
}
