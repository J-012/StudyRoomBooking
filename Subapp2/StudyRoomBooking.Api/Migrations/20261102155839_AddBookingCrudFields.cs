using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyRoomBooking.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingCrudFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudySessions_RoomId",
                table: "StudySessions");

            migrationBuilder.AlterColumn<string>(
                name: "Topic",
                table: "StudySessions",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrganizerUserId",
                table: "StudySessions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "StudySessions",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_OrganizerUserId",
                table: "StudySessions",
                column: "OrganizerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_RoomId_StartTime_EndTime",
                table: "StudySessions",
                columns: new[] { "RoomId", "StartTime", "EndTime" });

            migrationBuilder.AddForeignKey(
                name: "FK_StudySessions_AspNetUsers_OrganizerUserId",
                table: "StudySessions",
                column: "OrganizerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudySessions_AspNetUsers_OrganizerUserId",
                table: "StudySessions");

            migrationBuilder.DropIndex(
                name: "IX_StudySessions_OrganizerUserId",
                table: "StudySessions");

            migrationBuilder.DropIndex(
                name: "IX_StudySessions_RoomId_StartTime_EndTime",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "OrganizerUserId",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "StudySessions");

            migrationBuilder.AlterColumn<string>(
                name: "Topic",
                table: "StudySessions",
                type: "TEXT",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 200);

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_RoomId",
                table: "StudySessions",
                column: "RoomId");
        }
    }
}
