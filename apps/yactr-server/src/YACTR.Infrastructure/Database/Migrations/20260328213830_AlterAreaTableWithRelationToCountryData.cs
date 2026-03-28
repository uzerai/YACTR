using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YACTR.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AlterAreaTableWithRelationToCountryData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "country_id",
                table: "areas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_areas_country_id",
                table: "areas",
                column: "country_id");

            migrationBuilder.AddForeignKey(
                name: "fk_areas_country_data_country_id",
                table: "areas",
                column: "country_id",
                principalTable: "country_data",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_areas_country_data_country_id",
                table: "areas");

            migrationBuilder.DropIndex(
                name: "ix_areas_country_id",
                table: "areas");

            migrationBuilder.DropColumn(
                name: "country_id",
                table: "areas");
        }
    }
}
