using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyRoomBooking.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBuildingRelationToRooms : Migration
    {
        /// <inheritdoc />
            protected override void Up(MigrationBuilder migrationBuilder)
            {
                migrationBuilder.AddColumn<int>(
                    name: "BuildingId",
                    table: "Rooms",
                    type: "INTEGER",
                    nullable: true);

                migrationBuilder.Sql(
                    """
                    INSERT INTO Buildings (Name, Address)
                    SELECT DISTINCT r.Building, NULL
                    FROM Rooms r
                    WHERE TRIM(r.Building) <> ''
                    AND NOT EXISTS (
                        SELECT 1
                        FROM Buildings b
                        WHERE LOWER(b.Name) = LOWER(r.Building)
                    );
                    """
                );

                migrationBuilder.Sql(
                    """
                    UPDATE Rooms
                    SET BuildingId = (
                        SELECT b.Id
                        FROM Buildings b
                        WHERE LOWER(b.Name) = LOWER(Rooms.Building)
                        LIMIT 1
                    )
                    WHERE BuildingId IS NULL;
                    """
                );

                migrationBuilder.CreateIndex(
                    name: "IX_Rooms_BuildingId",
                    table: "Rooms",
                    column: "BuildingId");

                migrationBuilder.AddForeignKey(
                    name: "FK_Rooms_Buildings_BuildingId",
                    table: "Rooms",
                    column: "BuildingId",
                    principalTable: "Buildings",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Buildings_BuildingId",
                table: "Rooms");

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1,
                column: "BuildingId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2,
                column: "BuildingId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3,
                column: "BuildingId",
                value: null);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Buildings_BuildingId",
                table: "Rooms",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "Id");
        }
    }
}
