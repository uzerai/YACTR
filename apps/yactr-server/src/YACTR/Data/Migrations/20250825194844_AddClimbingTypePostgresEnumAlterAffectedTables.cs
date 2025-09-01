using Microsoft.EntityFrameworkCore.Migrations;
using YACTR.Data.Model.Climbing;

#nullable disable

namespace YACTR.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClimbingTypePostgresEnumAlterAffectedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:climbing_type", "sport,traditional,boulder,mixed,aid")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AddColumn<ClimbingType>(
                name: "type",
                table: "routes",
                type: "climbing_type",
                nullable: false,
                defaultValue: ClimbingType.Sport);

            migrationBuilder.DropColumn(
                name: "type",
                table: "pitches");

            migrationBuilder.AddColumn<ClimbingType>(
                name: "type",
                table: "pitches",
                type: "climbing_type",
                nullable: false,
                defaultValue: ClimbingType.Sport);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "type",
                table: "routes");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:Enum:climbing_type", "sport,traditional,boulder,mixed,aid")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.DropColumn(
                name: "type",
                table: "pitches");

            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "pitches",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
