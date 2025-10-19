using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YACTR.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiSectorImageSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_routes_images_sector_svg_overlay_id",
                table: "routes");

            migrationBuilder.DropForeignKey(
                name: "fk_sectors_images_sector_image_id",
                table: "sectors");

            migrationBuilder.DropIndex(
                name: "ix_sectors_sector_image_id",
                table: "sectors");

            migrationBuilder.DropIndex(
                name: "ix_routes_topo_image_id",
                table: "routes");

            migrationBuilder.RenameColumn(
                name: "sector_svg_overlay_id",
                table: "routes",
                newName: "sector_topo_image_overlay_svg_id");

            migrationBuilder.RenameIndex(
                name: "ix_routes_sector_svg_overlay_id",
                table: "routes",
                newName: "ix_routes_sector_topo_image_overlay_svg_id");

            migrationBuilder.AddColumn<Guid>(
                name: "primary_sector_image_id",
                table: "sectors",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "sector_topo_image_id",
                table: "routes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "sector_images",
                columns: table => new
                {
                    sector_id = table.Column<Guid>(type: "uuid", nullable: false),
                    image_id = table.Column<Guid>(type: "uuid", nullable: false),
                    order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sector_images", x => new { x.sector_id, x.image_id });
                    table.ForeignKey(
                        name: "fk_sector_images_images_image_id",
                        column: x => x.image_id,
                        principalTable: "images",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_sector_images_sectors_sector_id",
                        column: x => x.sector_id,
                        principalTable: "sectors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_sectors_primary_sector_image_id",
                table: "sectors",
                column: "primary_sector_image_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_routes_sector_topo_image_id",
                table: "routes",
                column: "sector_topo_image_id");

            migrationBuilder.CreateIndex(
                name: "ix_routes_topo_image_id",
                table: "routes",
                column: "topo_image_id");

            migrationBuilder.CreateIndex(
                name: "ix_sector_images_image_id",
                table: "sector_images",
                column: "image_id");

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
                name: "fk_sectors_images_primary_sector_image_id",
                table: "sectors",
                column: "primary_sector_image_id",
                principalTable: "images",
                principalColumn: "id");
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
                name: "fk_sectors_images_primary_sector_image_id",
                table: "sectors");

            migrationBuilder.DropTable(
                name: "sector_images");

            migrationBuilder.DropIndex(
                name: "ix_sectors_primary_sector_image_id",
                table: "sectors");

            migrationBuilder.DropIndex(
                name: "ix_routes_sector_topo_image_id",
                table: "routes");

            migrationBuilder.DropIndex(
                name: "ix_routes_topo_image_id",
                table: "routes");

            migrationBuilder.DropColumn(
                name: "primary_sector_image_id",
                table: "sectors");

            migrationBuilder.DropColumn(
                name: "sector_topo_image_id",
                table: "routes");

            migrationBuilder.RenameColumn(
                name: "sector_topo_image_overlay_svg_id",
                table: "routes",
                newName: "sector_svg_overlay_id");

            migrationBuilder.RenameIndex(
                name: "ix_routes_sector_topo_image_overlay_svg_id",
                table: "routes",
                newName: "ix_routes_sector_svg_overlay_id");

            migrationBuilder.CreateIndex(
                name: "ix_sectors_sector_image_id",
                table: "sectors",
                column: "sector_image_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_routes_topo_image_id",
                table: "routes",
                column: "topo_image_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_routes_images_sector_svg_overlay_id",
                table: "routes",
                column: "sector_svg_overlay_id",
                principalTable: "images",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_sectors_images_sector_image_id",
                table: "sectors",
                column: "sector_image_id",
                principalTable: "images",
                principalColumn: "id");
        }
    }
}
