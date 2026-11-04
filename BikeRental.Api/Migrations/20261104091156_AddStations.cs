using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BikeRental.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PickupStationId",
                table: "Rentals",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReturnStationId",
                table: "Rentals",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StationId",
                table: "Bikes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: false),
                    Longitude = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 1,
                column: "StationId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 2,
                column: "StationId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 3,
                column: "StationId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 4,
                column: "StationId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 5,
                column: "StationId",
                value: 3);

            migrationBuilder.InsertData(
                table: "Stations",
                columns: new[] { "Id", "Address", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 1, "Kauppapuistikko, 65100 Vaasa", 63.095999999999997, 21.6158, "Kauppatori" },
                    { 2, "Wolffintie 30, 65200 Vaasa", 63.104599999999998, 21.593499999999999, "VAMK Palosaari" },
                    { 3, "Hovioikeudenpuistikko 26, 65100 Vaasa", 63.0946, 21.620899999999999, "Rautatieasema" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_PickupStationId",
                table: "Rentals",
                column: "PickupStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_ReturnStationId",
                table: "Rentals",
                column: "ReturnStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Bikes_StationId",
                table: "Bikes",
                column: "StationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bikes_Stations_StationId",
                table: "Bikes",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_Stations_PickupStationId",
                table: "Rentals",
                column: "PickupStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Rentals_Stations_ReturnStationId",
                table: "Rentals",
                column: "ReturnStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bikes_Stations_StationId",
                table: "Bikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_Stations_PickupStationId",
                table: "Rentals");

            migrationBuilder.DropForeignKey(
                name: "FK_Rentals_Stations_ReturnStationId",
                table: "Rentals");

            migrationBuilder.DropTable(
                name: "Stations");

            migrationBuilder.DropIndex(
                name: "IX_Rentals_PickupStationId",
                table: "Rentals");

            migrationBuilder.DropIndex(
                name: "IX_Rentals_ReturnStationId",
                table: "Rentals");

            migrationBuilder.DropIndex(
                name: "IX_Bikes_StationId",
                table: "Bikes");

            migrationBuilder.DropColumn(
                name: "PickupStationId",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "ReturnStationId",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "StationId",
                table: "Bikes");
        }
    }
}
