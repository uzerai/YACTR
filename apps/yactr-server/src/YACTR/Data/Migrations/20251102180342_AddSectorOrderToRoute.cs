using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YACTR.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSectorOrderToRoute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "in_sector_order",
                table: "routes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "in_sector_order",
                table: "routes");
        }
    }
}
