using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudyRoomBooking.Mvc.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRoomSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 7);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Building", "Capacity", "RoomNumber" },
                values: new object[,]
                {
                    { 1, "Pilestredet 35", 6, "101" },
                    { 2, "Pilestredet 35", 6, "102" },
                    { 3, "Pilestredet 35", 12, "103" },
                    { 4, "Pilestredet 35", 6, "201" },
                    { 5, "Pilestredet 35", 6, "202" },
                    { 6, "Pilestredet 32", 8, "101" },
                    { 7, "Pilestredet 32", 8, "102" }
                });
        }
    }
}
