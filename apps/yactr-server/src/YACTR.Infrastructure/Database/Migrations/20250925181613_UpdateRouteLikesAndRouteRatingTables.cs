#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace YACTR.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRouteLikesAndRouteRatingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /// This whole file is a bit of a mishap; not sure what happened but noticed the changes when working
            /// on another migration. Leaving it as-is, since it's easier just to add the migration here and work
            /// forwards rather than rebuild backwards.
            migrationBuilder.DropForeignKey(
                name: "fk_route_like_routes_route_id",
                table: "route_like");

            migrationBuilder.DropForeignKey(
                name: "fk_route_like_users_user_id",
                table: "route_like");

            migrationBuilder.DropForeignKey(
                name: "fk_route_rating_routes_route_id",
                table: "route_rating");

            migrationBuilder.DropForeignKey(
                name: "fk_route_rating_users_user_id",
                table: "route_rating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_route_rating",
                table: "route_rating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_route_like",
                table: "route_like");

            migrationBuilder.RenameTable(
                name: "route_rating",
                newName: "route_ratings");

            migrationBuilder.RenameTable(
                name: "route_like",
                newName: "route_likes");

            migrationBuilder.RenameIndex(
                name: "ix_route_rating_user_id",
                table: "route_ratings",
                newName: "ix_route_ratings_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_route_rating_route_id",
                table: "route_ratings",
                newName: "ix_route_ratings_route_id");

            migrationBuilder.RenameIndex(
                name: "ix_route_like_user_id",
                table: "route_likes",
                newName: "ix_route_likes_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_route_like_route_id",
                table: "route_likes",
                newName: "ix_route_likes_route_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_route_ratings",
                table: "route_ratings",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_route_likes",
                table: "route_likes",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_route_likes_routes_route_id",
                table: "route_likes",
                column: "route_id",
                principalTable: "routes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_route_likes_users_user_id",
                table: "route_likes",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_route_ratings_routes_route_id",
                table: "route_ratings",
                column: "route_id",
                principalTable: "routes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_route_ratings_users_user_id",
                table: "route_ratings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_route_likes_routes_route_id",
                table: "route_likes");

            migrationBuilder.DropForeignKey(
                name: "fk_route_likes_users_user_id",
                table: "route_likes");

            migrationBuilder.DropForeignKey(
                name: "fk_route_ratings_routes_route_id",
                table: "route_ratings");

            migrationBuilder.DropForeignKey(
                name: "fk_route_ratings_users_user_id",
                table: "route_ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_route_ratings",
                table: "route_ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_route_likes",
                table: "route_likes");

            migrationBuilder.RenameTable(
                name: "route_ratings",
                newName: "route_rating");

            migrationBuilder.RenameTable(
                name: "route_likes",
                newName: "route_like");

            migrationBuilder.RenameIndex(
                name: "ix_route_ratings_user_id",
                table: "route_rating",
                newName: "ix_route_rating_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_route_ratings_route_id",
                table: "route_rating",
                newName: "ix_route_rating_route_id");

            migrationBuilder.RenameIndex(
                name: "ix_route_likes_user_id",
                table: "route_like",
                newName: "ix_route_like_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_route_likes_route_id",
                table: "route_like",
                newName: "ix_route_like_route_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_route_rating",
                table: "route_rating",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_route_like",
                table: "route_like",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_route_like_routes_route_id",
                table: "route_like",
                column: "route_id",
                principalTable: "routes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_route_like_users_user_id",
                table: "route_like",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_route_rating_routes_route_id",
                table: "route_rating",
                column: "route_id",
                principalTable: "routes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_route_rating_users_user_id",
                table: "route_rating",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
