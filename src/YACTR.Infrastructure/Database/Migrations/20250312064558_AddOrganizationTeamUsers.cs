#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace YACTR.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationTeamUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_organization_team_organizations_organization_id",
                table: "organization_team");

            migrationBuilder.DropPrimaryKey(
                name: "pk_organization_team",
                table: "organization_team");

            migrationBuilder.RenameTable(
                name: "organization_team",
                newName: "organization_teams");

            migrationBuilder.RenameIndex(
                name: "ix_organization_team_organization_id",
                table: "organization_teams",
                newName: "ix_organization_teams_organization_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_organization_teams",
                table: "organization_teams",
                column: "id");

            migrationBuilder.CreateTable(
                name: "organization_team_users",
                columns: table => new
                {
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_team_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_organization_team_users", x => new { x.organization_id, x.organization_team_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_organization_team_users_organization_teams_organization_tea",
                        column: x => x.organization_team_id,
                        principalTable: "organization_teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_organization_team_users_organization_users_organization_id_",
                        columns: x => new { x.organization_id, x.user_id },
                        principalTable: "organization_users",
                        principalColumns: new[] { "organization_id", "user_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_organization_team_users_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_organization_team_users_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_organization_team_users_organization_id_user_id",
                table: "organization_team_users",
                columns: new[] { "organization_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_organization_team_users_organization_team_id",
                table: "organization_team_users",
                column: "organization_team_id");

            migrationBuilder.CreateIndex(
                name: "ix_organization_team_users_user_id",
                table: "organization_team_users",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_organization_teams_organizations_organization_id",
                table: "organization_teams",
                column: "organization_id",
                principalTable: "organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_organization_teams_organizations_organization_id",
                table: "organization_teams");

            migrationBuilder.DropTable(
                name: "organization_team_users");

            migrationBuilder.DropPrimaryKey(
                name: "pk_organization_teams",
                table: "organization_teams");

            migrationBuilder.RenameTable(
                name: "organization_teams",
                newName: "organization_team");

            migrationBuilder.RenameIndex(
                name: "ix_organization_teams_organization_id",
                table: "organization_team",
                newName: "ix_organization_team_organization_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_organization_team",
                table: "organization_team",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_organization_team_organizations_organization_id",
                table: "organization_team",
                column: "organization_id",
                principalTable: "organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
