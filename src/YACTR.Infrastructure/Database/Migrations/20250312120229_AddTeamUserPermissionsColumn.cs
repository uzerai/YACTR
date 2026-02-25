#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamUserPermissionsColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ICollection<Permission>>(
                name: "permissions",
                table: "organization_team_users",
                type: "jsonb",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "permissions",
                table: "organization_team_users");
        }
    }
}
