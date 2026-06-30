using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mocny_RasberyPi_Images_Listener.Migrations
{
    /// <inheritdoc />
    public partial class AddIsClosedToSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                table: "Schedules",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClosed",
                table: "Schedules");
        }
    }
}
