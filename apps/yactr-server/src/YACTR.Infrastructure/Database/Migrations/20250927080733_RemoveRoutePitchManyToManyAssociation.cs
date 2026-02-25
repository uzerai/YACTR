#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace YACTR.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRoutePitchManyToManyAssociation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "route_pitches");

            migrationBuilder.AddColumn<Guid>(
                name: "route_id",
                table: "pitches",
                type: "uuid",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "ix_pitches_route_id",
                table: "pitches",
                column: "route_id");

            migrationBuilder.AddForeignKey(
                name: "fk_pitches_routes_route_id",
                table: "pitches",
                column: "route_id",
                principalTable: "routes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pitches_routes_route_id",
                table: "pitches");

            migrationBuilder.DropIndex(
                name: "ix_pitches_route_id",
                table: "pitches");

            migrationBuilder.DropColumn(
                name: "route_id",
                table: "pitches");

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
    }
}
