using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BikeRental.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDO7IAsxiuY3CK48XjKrNBPFYgo9owSCt0Zi9jDJqjCf6tHhVeQkQS2TX9nbUBOD+w==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEP8YpShRA5TmKFSjYIC8/4iyD2DaB/Y3iVq82EXvujZEku8U9+T71ly4qqw+XDz4DA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFJB5Jvd7gioD9l0/n06z0vb+Q5cO3AMi+wrva+7ZyEd6uiv9STbc+T2uP+5DGOwnQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "not-set");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "not-set");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "not-set");
        }
    }
}
