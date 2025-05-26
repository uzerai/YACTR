using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet.PlaygroundMigrations
{
    /// <inheritdoc />
    public partial class RenameSectorRouteToSectorApproachPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "route",
                table: "sectors",
                newName: "approach_path");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "approach_path",
                table: "sectors",
                newName: "route");
        }
    }
}
