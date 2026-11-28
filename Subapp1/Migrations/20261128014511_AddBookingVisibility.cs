using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyRoomBooking.Mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingVisibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Bookings",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Bookings");
        }
    }
}
