using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using YACTR.Data.Model.Climbing.Topo;

#nullable disable

namespace YACTR.Data.Migrations
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
