#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using YACTR.Domain.Model.Climbing.Topo;

namespace YACTR.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddTopoLinePointsAndPitchHeights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<TopoLinePoint>>(
                name: "topo_line_points",
                table: "routes",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "height",
                table: "pitches",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "topo_line_points",
                table: "routes");

            migrationBuilder.DropColumn(
                name: "height",
                table: "pitches");
        }
    }
}
