#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

namespace YACTR.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class RefactorAscents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ascents_pitches_pitch_id",
                table: "ascents");

            migrationBuilder.DropPrimaryKey(
                name: "pk_ascents",
                table: "ascents");

            migrationBuilder.DropIndex(
                name: "ix_ascents_pitch_id",
                table: "ascents");

            migrationBuilder.DropColumn(
                name: "ascent_type",
                table: "ascents");

            migrationBuilder.DropColumn(
                name: "pitch_id",
                table: "ascents");

            migrationBuilder.AlterColumn<Guid>(
                name: "route_id",
                table: "ascents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<Instant>(
                name: "deleted_at",
                table: "ascents",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Instant>(
                name: "updated_at",
                table: "ascents",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ascents",
                table: "ascents",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ascents",
                table: "ascents");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "ascents");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "ascents");

            migrationBuilder.AlterColumn<Guid>(
                name: "route_id",
                table: "ascents",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "ascent_type",
                table: "ascents",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "pitch_id",
                table: "ascents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_ascents",
                table: "ascents",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_ascents_pitch_id",
                table: "ascents",
                column: "pitch_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ascents_pitches_pitch_id",
                table: "ascents",
                column: "pitch_id",
                principalTable: "pitches",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
