using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyRoomBooking.Mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddBuildingsAndAmenities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BuildingId",
                table: "Rooms",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Amenities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amenities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoomAmenities",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    AmenityId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomAmenities", x => new { x.RoomId, x.AmenityId });
                    table.ForeignKey(
                        name: "FK_RoomAmenities_Amenities_AmenityId",
                        column: x => x.AmenityId,
                        principalTable: "Amenities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomAmenities_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Preserve every existing room's building, including names created by admins.
            // Keep the original seeded room/building IDs aligned with the model snapshot.
            migrationBuilder.Sql("INSERT INTO Buildings (Id, Name) SELECT 1, 'Pilestredet 35' WHERE EXISTS (SELECT 1 FROM Rooms WHERE Building = 'Pilestredet 35');");
            migrationBuilder.Sql("INSERT INTO Buildings (Id, Name) SELECT 2, 'Pilestredet 32' WHERE EXISTS (SELECT 1 FROM Rooms WHERE Building = 'Pilestredet 32');");
            migrationBuilder.Sql("INSERT INTO Buildings (Name) SELECT DISTINCT Building FROM Rooms WHERE Building IS NOT NULL AND TRIM(Building) <> '' AND NOT EXISTS (SELECT 1 FROM Buildings WHERE Name = Rooms.Building);");
            migrationBuilder.Sql("UPDATE Rooms SET BuildingId = (SELECT Id FROM Buildings WHERE Name = Rooms.Building) WHERE Building IS NOT NULL AND TRIM(Building) <> '';");
            // Older data may contain an empty building name. Give those rooms a valid location.
            migrationBuilder.Sql("INSERT INTO Buildings (Name) SELECT 'Unassigned' WHERE EXISTS (SELECT 1 FROM Rooms WHERE BuildingId = 0) AND NOT EXISTS (SELECT 1 FROM Buildings WHERE Name = 'Unassigned');");
            migrationBuilder.Sql("UPDATE Rooms SET BuildingId = (SELECT Id FROM Buildings WHERE Name = 'Unassigned') WHERE BuildingId = 0;");

            migrationBuilder.DropColumn(name: "Building", table: "Rooms");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_BuildingId",
                table: "Rooms",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Amenities_Name",
                table: "Amenities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_Name",
                table: "Buildings",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomAmenities_AmenityId",
                table: "RoomAmenities",
                column: "AmenityId");

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

            migrationBuilder.DropTable(
                name: "RoomAmenities");

            migrationBuilder.DropTable(
                name: "Amenities");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_BuildingId",
                table: "Rooms");

            migrationBuilder.AddColumn<string>(
                name: "Building",
                table: "Rooms",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE Rooms SET Building = (SELECT Name FROM Buildings WHERE Id = Rooms.BuildingId);");
            migrationBuilder.DropColumn(name: "BuildingId", table: "Rooms");
            migrationBuilder.DropTable(name: "Buildings");
        }
    }
}
