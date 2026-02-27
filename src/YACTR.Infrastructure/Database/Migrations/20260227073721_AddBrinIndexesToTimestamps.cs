using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YACTR.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddBrinIndexesToTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_auth0_user_id",
                table: "users");

            migrationBuilder.CreateIndex(
                name: "ix_users_auth0_user_id",
                table: "users",
                column: "auth0_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_created_at",
                table: "users",
                column: "created_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_users_updated_at",
                table: "users",
                column: "updated_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_sectors_created_at",
                table: "sectors",
                column: "created_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_sectors_updated_at",
                table: "sectors",
                column: "updated_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_routes_created_at",
                table: "routes",
                column: "created_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_routes_updated_at",
                table: "routes",
                column: "updated_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_route_ratings_created_at",
                table: "route_ratings",
                column: "created_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_route_ratings_updated_at",
                table: "route_ratings",
                column: "updated_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_route_likes_created_at",
                table: "route_likes",
                column: "created_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_route_likes_updated_at",
                table: "route_likes",
                column: "updated_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_pitches_created_at",
                table: "pitches",
                column: "created_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_pitches_updated_at",
                table: "pitches",
                column: "updated_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_organizations_created_at",
                table: "organizations",
                column: "created_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_organizations_updated_at",
                table: "organizations",
                column: "updated_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_organization_teams_created_at",
                table: "organization_teams",
                column: "created_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_organization_teams_updated_at",
                table: "organization_teams",
                column: "updated_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_images_created_at",
                table: "images",
                column: "created_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_images_updated_at",
                table: "images",
                column: "updated_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_ascents_created_at",
                table: "ascents",
                column: "created_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_ascents_updated_at",
                table: "ascents",
                column: "updated_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_areas_created_at",
                table: "areas",
                column: "created_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");

            migrationBuilder.CreateIndex(
                name: "IX_areas_updated_at",
                table: "areas",
                column: "updated_at",
                descending: new bool[0])
                .Annotation("Npgsql:IndexMethod", "brin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_users_auth0_user_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_created_at",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_updated_at",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_sectors_created_at",
                table: "sectors");

            migrationBuilder.DropIndex(
                name: "IX_sectors_updated_at",
                table: "sectors");

            migrationBuilder.DropIndex(
                name: "IX_routes_created_at",
                table: "routes");

            migrationBuilder.DropIndex(
                name: "IX_routes_updated_at",
                table: "routes");

            migrationBuilder.DropIndex(
                name: "IX_route_ratings_created_at",
                table: "route_ratings");

            migrationBuilder.DropIndex(
                name: "IX_route_ratings_updated_at",
                table: "route_ratings");

            migrationBuilder.DropIndex(
                name: "IX_route_likes_created_at",
                table: "route_likes");

            migrationBuilder.DropIndex(
                name: "IX_route_likes_updated_at",
                table: "route_likes");

            migrationBuilder.DropIndex(
                name: "IX_pitches_created_at",
                table: "pitches");

            migrationBuilder.DropIndex(
                name: "IX_pitches_updated_at",
                table: "pitches");

            migrationBuilder.DropIndex(
                name: "IX_organizations_created_at",
                table: "organizations");

            migrationBuilder.DropIndex(
                name: "IX_organizations_updated_at",
                table: "organizations");

            migrationBuilder.DropIndex(
                name: "IX_organization_teams_created_at",
                table: "organization_teams");

            migrationBuilder.DropIndex(
                name: "IX_organization_teams_updated_at",
                table: "organization_teams");

            migrationBuilder.DropIndex(
                name: "IX_images_created_at",
                table: "images");

            migrationBuilder.DropIndex(
                name: "IX_images_updated_at",
                table: "images");

            migrationBuilder.DropIndex(
                name: "IX_ascents_created_at",
                table: "ascents");

            migrationBuilder.DropIndex(
                name: "IX_ascents_updated_at",
                table: "ascents");

            migrationBuilder.DropIndex(
                name: "IX_areas_created_at",
                table: "areas");

            migrationBuilder.DropIndex(
                name: "IX_areas_updated_at",
                table: "areas");

            migrationBuilder.CreateIndex(
                name: "ix_users_auth0_user_id",
                table: "users",
                column: "auth0_user_id",
                unique: true);
        }
    }
}
