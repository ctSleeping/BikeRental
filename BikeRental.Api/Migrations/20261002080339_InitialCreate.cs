using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BikeRental.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Model = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    PricePerHour = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    InMaintenance = table.Column<bool>(type: "INTEGER", nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bikes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Bikes",
                columns: new[] { "Id", "DeletedUtc", "InMaintenance", "Model", "PricePerHour", "Type" },
                values: new object[,]
                {
                    { 1, null, false, "Helkama Jopo", 3.50m, "City" },
                    { 2, null, false, "Trek Marlin 7", 5.00m, "Mountain" },
                    { 3, null, false, "Tunturi E-Bike", 8.00m, "Electric" },
                    { 4, null, false, "Puky ZL 16", 2.00m, "Kids" },
                    { 5, null, true, "Cannondale Quick", 4.00m, "City" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bikes");
        }
    }
}
