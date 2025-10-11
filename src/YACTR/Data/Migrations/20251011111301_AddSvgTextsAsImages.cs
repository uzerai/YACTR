using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YACTR.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSvgTextsAsImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sector_image_overlay_svg",
                table: "routes");

            migrationBuilder.DropColumn(
                name: "topo_image_overlay_svg",
                table: "routes");

            migrationBuilder.DropColumn(
                name: "svg_topo_image_overlay",
                table: "pitches");

            migrationBuilder.AddColumn<Guid>(
                name: "sector_svg_overlay_id",
                table: "routes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "topo_image_overlay_svg_id",
                table: "routes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "route_svg_overlay_id",
                table: "pitches",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_routes_sector_svg_overlay_id",
                table: "routes",
                column: "sector_svg_overlay_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_routes_topo_image_overlay_svg_id",
                table: "routes",
                column: "topo_image_overlay_svg_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_pitches_route_svg_overlay_id",
                table: "pitches",
                column: "route_svg_overlay_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_pitches_images_route_svg_overlay_id",
                table: "pitches",
                column: "route_svg_overlay_id",
                principalTable: "images",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_routes_images_sector_svg_overlay_id",
                table: "routes",
                column: "sector_svg_overlay_id",
                principalTable: "images",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_routes_images_topo_image_overlay_svg_id",
                table: "routes",
                column: "topo_image_overlay_svg_id",
                principalTable: "images",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_pitches_images_route_svg_overlay_id",
                table: "pitches");

            migrationBuilder.DropForeignKey(
                name: "fk_routes_images_sector_svg_overlay_id",
                table: "routes");

            migrationBuilder.DropForeignKey(
                name: "fk_routes_images_topo_image_overlay_svg_id",
                table: "routes");

            migrationBuilder.DropIndex(
                name: "ix_routes_sector_svg_overlay_id",
                table: "routes");

            migrationBuilder.DropIndex(
                name: "ix_routes_topo_image_overlay_svg_id",
                table: "routes");

            migrationBuilder.DropIndex(
                name: "ix_pitches_route_svg_overlay_id",
                table: "pitches");

            migrationBuilder.DropColumn(
                name: "sector_svg_overlay_id",
                table: "routes");

            migrationBuilder.DropColumn(
                name: "topo_image_overlay_svg_id",
                table: "routes");

            migrationBuilder.DropColumn(
                name: "route_svg_overlay_id",
                table: "pitches");

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

            migrationBuilder.AddColumn<string>(
                name: "svg_topo_image_overlay",
                table: "pitches",
                type: "text",
                nullable: true);
        }
    }
}
