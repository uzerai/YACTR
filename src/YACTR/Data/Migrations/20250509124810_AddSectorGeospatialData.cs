using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Dotnet.PlaygroundMigrations
{
    /// <inheritdoc />
    public partial class AddSectorGeospatialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Point>(
                name: "entry_point",
                table: "sectors",
                type: "geometry",
                nullable: false);

            migrationBuilder.AddColumn<Point>(
                name: "recommended_parking_location",
                table: "sectors",
                type: "geometry",
                nullable: true);

            migrationBuilder.AddColumn<LineString>(
                name: "route",
                table: "sectors",
                type: "geometry",
                nullable: true);

            migrationBuilder.AddColumn<Polygon>(
                name: "sector_area",
                table: "sectors",
                type: "geometry",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "entry_point",
                table: "sectors");

            migrationBuilder.DropColumn(
                name: "recommended_parking_location",
                table: "sectors");

            migrationBuilder.DropColumn(
                name: "route",
                table: "sectors");

            migrationBuilder.DropColumn(
                name: "sector_area",
                table: "sectors");
        }
    }
}
