using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Dotnet.PlaygroundMigrations
{
    /// <inheritdoc />
    public partial class AddSectorEntityAndRemoveNoteEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pitches_crags_crag_id",
                table: "pitches");

            migrationBuilder.DropForeignKey(
                name: "fk_routes_crags_crag_id",
                table: "routes");

            migrationBuilder.DropTable(
                name: "notes");

            migrationBuilder.RenameColumn(
                name: "crag_id",
                table: "routes",
                newName: "sector_id");

            migrationBuilder.RenameIndex(
                name: "ix_routes_crag_id",
                table: "routes",
                newName: "ix_routes_sector_id");

            migrationBuilder.RenameColumn(
                name: "crag_id",
                table: "pitches",
                newName: "sector_id");

            migrationBuilder.RenameIndex(
                name: "ix_pitches_crag_id",
                table: "pitches",
                newName: "ix_pitches_sector_id");

            migrationBuilder.CreateTable(
                name: "sectors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    crag_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sectors", x => x.id);
                    table.ForeignKey(
                        name: "fk_sectors_crags_crag_id",
                        column: x => x.crag_id,
                        principalTable: "crags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_sectors_crag_id",
                table: "sectors",
                column: "crag_id");

            migrationBuilder.AddForeignKey(
                name: "fk_pitches_sectors_sector_id",
                table: "pitches",
                column: "sector_id",
                principalTable: "sectors",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_routes_sectors_sector_id",
                table: "routes",
                column: "sector_id",
                principalTable: "sectors",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pitches_sectors_sector_id",
                table: "pitches");

            migrationBuilder.DropForeignKey(
                name: "fk_routes_sectors_sector_id",
                table: "routes");

            migrationBuilder.DropTable(
                name: "sectors");

            migrationBuilder.RenameColumn(
                name: "sector_id",
                table: "routes",
                newName: "crag_id");

            migrationBuilder.RenameIndex(
                name: "ix_routes_sector_id",
                table: "routes",
                newName: "ix_routes_crag_id");

            migrationBuilder.RenameColumn(
                name: "sector_id",
                table: "pitches",
                newName: "crag_id");

            migrationBuilder.RenameIndex(
                name: "ix_pitches_sector_id",
                table: "pitches",
                newName: "ix_pitches_crag_id");

            migrationBuilder.CreateTable(
                name: "notes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    updated_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    crag_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notes", x => x.id);
                    table.ForeignKey(
                        name: "fk_notes_crags_crag_id",
                        column: x => x.crag_id,
                        principalTable: "crags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_notes_users_author_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_notes_author_id",
                table: "notes",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_notes_crag_id",
                table: "notes",
                column: "crag_id");

            migrationBuilder.AddForeignKey(
                name: "fk_pitches_crags_crag_id",
                table: "pitches",
                column: "crag_id",
                principalTable: "crags",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_routes_crags_crag_id",
                table: "routes",
                column: "crag_id",
                principalTable: "crags",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
