using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social_Network.API.Migrations
{
    /// <inheritdoc />
    public partial class FixNotificationsTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotificatonsEnabled",
                table: "UserSettings",
                newName: "NotificationsEnabled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NotificationsEnabled",
                table: "UserSettings",
                newName: "NotificatonsEnabled");
        }
    }
}
