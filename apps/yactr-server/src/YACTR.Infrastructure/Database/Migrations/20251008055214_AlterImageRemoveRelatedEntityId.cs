#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace YACTR.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AlterImageRemoveRelatedEntityId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "related_entity_id",
                table: "images");

            migrationBuilder.AddColumn<Guid>(
                name: "sector_image_id",
                table: "sectors",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_sectors_sector_image_id",
                table: "sectors",
                column: "sector_image_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_sectors_images_sector_image_id",
                table: "sectors",
                column: "sector_image_id",
                principalTable: "images",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sectors_images_sector_image_id",
                table: "sectors");

            migrationBuilder.DropIndex(
                name: "ix_sectors_sector_image_id",
                table: "sectors");

            migrationBuilder.DropColumn(
                name: "sector_image_id",
                table: "sectors");

            migrationBuilder.AddColumn<Guid>(
                name: "related_entity_id",
                table: "images",
                type: "uuid",
                nullable: true);
        }
    }
}
