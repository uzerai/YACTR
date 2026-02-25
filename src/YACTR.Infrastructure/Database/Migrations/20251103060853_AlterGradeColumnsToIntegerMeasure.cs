#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace YACTR.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AlterGradeColumnsToIntegerMeasure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "grade",
                table: "routes");

            migrationBuilder.AddColumn<int>(
                name: "grade",
                table: "routes",
                type: "integer",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "gear_count",
                table: "pitches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "grade",
                table: "pitches",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "gear_count",
                table: "pitches");

            migrationBuilder.DropColumn(
                name: "grade",
                table: "pitches");

            migrationBuilder.DropColumn(
                name: "grade",
                table: "routes"
            );

            migrationBuilder.AddColumn<string>(
                name: "grade",
                table: "routes",
                type: "text",
                nullable: true
            );
        }
    }
}
