using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet.PlaygroundMigrations
{
    /// <inheritdoc />
    public partial class AddManyToManyPitchesToRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pitch_route");

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "pitches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "route_pitches",
                columns: table => new
                {
                    route_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pitch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pitch_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_route_pitches", x => new { x.route_id, x.pitch_id });
                    table.ForeignKey(
                        name: "fk_route_pitches_pitches_pitch_id",
                        column: x => x.pitch_id,
                        principalTable: "pitches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_route_pitches_routes_route_id",
                        column: x => x.route_id,
                        principalTable: "routes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_route_pitches_pitch_id",
                table: "route_pitches",
                column: "pitch_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "route_pitches");

            migrationBuilder.DropColumn(
                name: "type",
                table: "pitches");

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
                name: "ix_pitch_route_routes_id",
                table: "pitch_route",
                column: "routes_id");
        }
    }
}
