using Microsoft.EntityFrameworkCore.Migrations;

using NetTopologySuite.Geometries;

#nullable disable

namespace YACTR.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AlterAreaEntryPointToNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "entry_point",
                table: "sectors",
                type: "geometry",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geometry");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "entry_point",
                table: "sectors",
                type: "geometry",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry",
                oldNullable: true);
        }
    }
}
