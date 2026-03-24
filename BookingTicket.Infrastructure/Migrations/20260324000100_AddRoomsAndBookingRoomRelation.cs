using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingTicket.Infrastructure.Migrations
{
    public partial class AddRoomsAndBookingRoomRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SeatCapacity = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            var defaultRoomId = new Guid("11111111-1111-1111-1111-111111111111");
            var defaultAuditDate = new DateTimeOffset(new DateTime(2026, 3, 24), TimeSpan.Zero);

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Name", "SeatCapacity", "CreatedAtUtc", "CreatedBy", "LastModifiedUtc", "LastModifiedBy" },
                values: new object[]
                {
                    defaultRoomId,
                    "Default Room",
                    100,
                    defaultAuditDate,
                    "migration",
                    defaultAuditDate,
                    "migration"
                });

            migrationBuilder.AddColumn<Guid>(
                name: "RoomId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.Sql("UPDATE [Bookings] SET [RoomId] = '11111111-1111-1111-1111-111111111111' WHERE [RoomId] IS NULL");

            migrationBuilder.AlterColumn<Guid>(
                name: "RoomId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Rooms_RoomId",
                table: "Bookings",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Rooms_RoomId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_RoomId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "Rooms");
        }
    }
}
