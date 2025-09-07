
using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Climbing.Rating;

namespace YACTR.Data.ConfigurationExtension;

public static class RouteRatingConfigurationExtension
{
    public static ModelBuilder ConfigureRouteRatingModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RouteRating>()
            .HasOne(e => e.User)
            .WithMany(r => r.RouteRatings)
            .HasForeignKey(e => e.UserId);

        modelBuilder.Entity<RouteRating>()
            .HasOne(e => e.Route)
            .WithMany(r => r.RouteRatings)
            .HasForeignKey(e => e.RouteId);

        return modelBuilder;
    }
} 