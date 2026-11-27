using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyRoomBooking.Mvc.Migrations
{
    /// <inheritdoc />
    public partial class UseCourseCodeAsPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Courses_CourseId",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "CourseCode",
                table: "Bookings",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            // Preserve the course assigned to existing bookings before the numeric key is removed.
            migrationBuilder.Sql("""
                UPDATE Bookings
                SET CourseCode = (
                    SELECT CourseCode
                    FROM Courses
                    WHERE Courses.Id = Bookings.CourseId
                )
                WHERE CourseId IS NOT NULL;
                """);

            migrationBuilder.DropPrimaryKey(
                name: "PK_Courses",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_CourseId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Bookings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Courses",
                table: "Courses",
                column: "CourseCode");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CourseCode",
                table: "Bookings",
                column: "CourseCode");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Courses_CourseCode",
                table: "Bookings",
                column: "CourseCode",
                principalTable: "Courses",
                principalColumn: "CourseCode",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Courses_CourseCode",
                table: "Bookings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Courses",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_CourseCode",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CourseCode",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Courses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Bookings",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Courses",
                table: "Courses",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseCode",
                table: "Courses",
                column: "CourseCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CourseId",
                table: "Bookings",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Courses_CourseId",
                table: "Bookings",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
