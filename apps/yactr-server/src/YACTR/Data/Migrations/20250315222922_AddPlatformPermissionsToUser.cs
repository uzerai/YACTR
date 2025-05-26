using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using YACTR.Data.Model.Authorization.Permissions;

#nullable disable

namespace YACTR.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlatformPermissionsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ICollection<Permission>>(
                name: "platform_permissions",
                table: "users",
                type: "jsonb",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "platform_permissions",
                table: "users");
        }
    }
}
