using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Dotnet.PlaygroundMigrations
{
    /// <inheritdoc />
    public partial class RenameCragModelToArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sectors_crags_crag_id",
                table: "sectors");

            migrationBuilder.DropTable(
                name: "crags");

            migrationBuilder.RenameColumn(
                name: "crag_id",
                table: "sectors",
                newName: "area_id");

            migrationBuilder.RenameIndex(
                name: "ix_sectors_crag_id",
                table: "sectors",
                newName: "ix_sectors_area_id");

            migrationBuilder.CreateTable(
                name: "areas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    location = table.Column<string>(type: "text", nullable: false),
                    maintainer_organization_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_areas", x => x.id);
                    table.ForeignKey(
                        name: "fk_areas_organizations_maintainer_organization_id",
                        column: x => x.maintainer_organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_areas_maintainer_organization_id",
                table: "areas",
                column: "maintainer_organization_id");

            migrationBuilder.AddForeignKey(
                name: "fk_sectors_areas_area_id",
                table: "sectors",
                column: "area_id",
                principalTable: "areas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sectors_areas_area_id",
                table: "sectors");

            migrationBuilder.DropTable(
                name: "areas");

            migrationBuilder.RenameColumn(
                name: "area_id",
                table: "sectors",
                newName: "crag_id");

            migrationBuilder.RenameIndex(
                name: "ix_sectors_area_id",
                table: "sectors",
                newName: "ix_sectors_crag_id");

            migrationBuilder.CreateTable(
                name: "crags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    maintainer_organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    location = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_crags", x => x.id);
                    table.ForeignKey(
                        name: "fk_crags_organizations_maintainer_organization_id",
                        column: x => x.maintainer_organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_crags_maintainer_organization_id",
                table: "crags",
                column: "maintainer_organization_id");

            migrationBuilder.AddForeignKey(
                name: "fk_sectors_crags_crag_id",
                table: "sectors",
                column: "crag_id",
                principalTable: "crags",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
