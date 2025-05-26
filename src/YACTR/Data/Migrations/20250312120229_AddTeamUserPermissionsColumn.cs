using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using YACTR.Model.Authorization.Permissions;

#nullable disable

namespace YACTR.Migrations
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
