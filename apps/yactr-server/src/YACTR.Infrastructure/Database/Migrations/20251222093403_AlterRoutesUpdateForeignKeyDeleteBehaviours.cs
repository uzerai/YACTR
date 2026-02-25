#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace YACTR.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AlterRoutesUpdateForeignKeyDeleteBehaviours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_routes_images_sector_topo_image_id",
                table: "routes");

            migrationBuilder.DropForeignKey(
                name: "fk_routes_images_sector_topo_image_overlay_svg_id",
                table: "routes");

            migrationBuilder.DropForeignKey(
                name: "fk_routes_images_topo_image_id",
                table: "routes");

            migrationBuilder.DropForeignKey(
                name: "fk_routes_images_topo_image_overlay_svg_id",
                table: "routes");

            migrationBuilder.AddForeignKey(
                name: "fk_routes_images_sector_topo_image_id",
                table: "routes",
                column: "sector_topo_image_id",
                principalTable: "images",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_routes_images_sector_topo_image_overlay_svg_id",
                table: "routes",
                column: "sector_topo_image_overlay_svg_id",
                principalTable: "images",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_routes_images_topo_image_id",
                table: "routes",
                column: "topo_image_id",
                principalTable: "images",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_routes_images_topo_image_overlay_svg_id",
                table: "routes",
                column: "topo_image_overlay_svg_id",
                principalTable: "images",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_routes_images_sector_topo_image_id",
                table: "routes");

            migrationBuilder.DropForeignKey(
                name: "fk_routes_images_sector_topo_image_overlay_svg_id",
                table: "routes");

            migrationBuilder.DropForeignKey(
                name: "fk_routes_images_topo_image_id",
                table: "routes");

            migrationBuilder.DropForeignKey(
                name: "fk_routes_images_topo_image_overlay_svg_id",
                table: "routes");

            migrationBuilder.AddForeignKey(
                name: "fk_routes_images_sector_topo_image_id",
                table: "routes",
                column: "sector_topo_image_id",
                principalTable: "images",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_routes_images_sector_topo_image_overlay_svg_id",
                table: "routes",
                column: "sector_topo_image_overlay_svg_id",
                principalTable: "images",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_routes_images_topo_image_id",
                table: "routes",
                column: "topo_image_id",
                principalTable: "images",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_routes_images_topo_image_overlay_svg_id",
                table: "routes",
                column: "topo_image_overlay_svg_id",
                principalTable: "images",
                principalColumn: "id");
        }
    }
}
