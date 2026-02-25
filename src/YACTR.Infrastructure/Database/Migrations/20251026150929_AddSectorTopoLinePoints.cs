#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using YACTR.Domain.Model.Climbing.Topo;

namespace YACTR.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSectorTopoLinePoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<TopoLinePoint>>(
                name: "sector_topo_line_points",
                table: "routes",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sector_topo_line_points",
                table: "routes");
        }
    }
}
