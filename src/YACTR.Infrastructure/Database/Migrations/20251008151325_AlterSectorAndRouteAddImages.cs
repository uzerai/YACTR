#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace YACTR.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AlterSectorAndRouteAddImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "sector_image_overlay_svg",
                table: "routes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "topo_image_overlay_svg",
                table: "routes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sector_image_overlay_svg",
                table: "routes");

            migrationBuilder.DropColumn(
                name: "topo_image_overlay_svg",
                table: "routes");
        }
    }
}
