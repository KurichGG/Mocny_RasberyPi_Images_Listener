using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mocny_RasberyPi_Images_Listener.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActivatedToSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActivated",
                table: "Schedules",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActivated",
                table: "Schedules");
        }
    }
}
