using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Dotnet.PlaygroundMigrations
{
    /// <inheritdoc />
    public partial class AddClimbingRelatedModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "crags",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    location = table.Column<string>(type: "text", nullable: false),
                    how_to_get_there = table.Column<string>(type: "text", nullable: true),
                    maintainer_organization_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_crags", x => x.id);
                    table.ForeignKey(
                        name: "fk_crags_organizations_maintainer_organization_id",
                        column: x => x.maintainer_organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    title = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    crag_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notes", x => x.id);
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

            migrationBuilder.CreateTable(
                name: "pitches",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    crag_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pitches", x => x.id);
                    table.ForeignKey(
                        name: "fk_pitches_crags_crag_id",
                        column: x => x.crag_id,
                        principalTable: "crags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "routes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    grade = table.Column<string>(type: "text", nullable: false),
                    first_ascent_date = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    first_ascent_climber_name = table.Column<string>(type: "text", nullable: true),
                    bolter_name = table.Column<string>(type: "text", nullable: true),
                    crag_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_routes", x => x.id);
                    table.ForeignKey(
                        name: "fk_routes_crags_crag_id",
                        column: x => x.crag_id,
                        principalTable: "crags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pitch_route",
                columns: table => new
                {
                    pitches_id = table.Column<Guid>(type: "uuid", nullable: false),
                    routes_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pitch_route", x => new { x.pitches_id, x.routes_id });
                    table.ForeignKey(
                        name: "fk_pitch_route_pitches_pitches_id",
                        column: x => x.pitches_id,
                        principalTable: "pitches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_pitch_route_routes_routes_id",
                        column: x => x.routes_id,
                        principalTable: "routes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_crags_maintainer_organization_id",
                table: "crags",
                column: "maintainer_organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_notes_author_id",
                table: "notes",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_notes_crag_id",
                table: "notes",
                column: "crag_id");

            migrationBuilder.CreateIndex(
                name: "ix_pitch_route_routes_id",
                table: "pitch_route",
                column: "routes_id");

            migrationBuilder.CreateIndex(
                name: "ix_pitches_crag_id",
                table: "pitches",
                column: "crag_id");

            migrationBuilder.CreateIndex(
                name: "ix_routes_crag_id",
                table: "routes",
                column: "crag_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notes");

            migrationBuilder.DropTable(
                name: "pitch_route");

            migrationBuilder.DropTable(
                name: "pitches");

            migrationBuilder.DropTable(
                name: "routes");

            migrationBuilder.DropTable(
                name: "crags");
        }
    }
}
