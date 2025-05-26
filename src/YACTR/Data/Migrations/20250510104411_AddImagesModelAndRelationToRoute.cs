using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace Dotnet.PlaygroundMigrations
{
    /// <inheritdoc />
    public partial class AddImagesModelAndRelationToRoute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "topo_image_id",
                table: "routes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    key = table.Column<string>(type: "text", nullable: false),
                    bucket = table.Column<string>(type: "text", nullable: false),
                    url = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    uploader_id = table.Column<Guid>(type: "uuid", nullable: true),
                    related_entity_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_images_users_uploader_id",
                        column: x => x.uploader_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_routes_topo_image_id",
                table: "routes",
                column: "topo_image_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_images_uploader_id",
                table: "images",
                column: "uploader_id");

            migrationBuilder.AddForeignKey(
                name: "fk_routes_images_topo_image_id",
                table: "routes",
                column: "topo_image_id",
                principalTable: "images",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_routes_images_topo_image_id",
                table: "routes");

            migrationBuilder.DropTable(
                name: "images");

            migrationBuilder.DropIndex(
                name: "ix_routes_topo_image_id",
                table: "routes");

            migrationBuilder.DropColumn(
                name: "topo_image_id",
                table: "routes");
        }
    }
}
