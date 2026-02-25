#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AlterOrganizationPermissionsToJsonColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "organization_permissions");

            migrationBuilder.AddColumn<ICollection<Permission>>(
                name: "permissions",
                table: "organization_users",
                type: "jsonb",
                defaultValue: new List<Permission>(),
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "ix_organization_users_permissions",
                table: "organization_users",
                column: "permissions")
                .Annotation("Npgsql:IndexMethod", "gin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_organization_users_permissions",
                table: "organization_users");

            migrationBuilder.DropColumn(
                name: "permissions",
                table: "organization_users");

            migrationBuilder.CreateTable(
                name: "organization_permissions",
                columns: table => new
                {
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_permissions", x => new { x.organization_id, x.user_id, x.permission });
                    table.ForeignKey(
                        name: "fk_organization_permissions_organization_users_organization_id",
                        columns: x => new { x.organization_id, x.user_id },
                        principalTable: "organization_users",
                        principalColumns: new[] { "organization_id", "user_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_organization_permissions_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_organization_permissions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_organization_permissions_user_id",
                table: "organization_permissions",
                column: "user_id");
        }
    }
}
