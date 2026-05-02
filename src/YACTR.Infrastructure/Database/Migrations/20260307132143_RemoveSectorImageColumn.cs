using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YACTR.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSectorImageColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sector_image_id",
                table: "sectors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "sector_image_id",
                table: "sectors",
                type: "uuid",
                nullable: true);
        }
    }
}
