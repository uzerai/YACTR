
using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Climbing.Rating;

namespace YACTR.Data.ConfigurationExtension;

public static class RouteLikeConfigurationExtension
{
    public static ModelBuilder ConfigureRouteLikeModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RouteLike>()
            .HasOne(e => e.User)
            .WithMany(r => r.RouteLikes)
            .HasForeignKey(e => e.UserId);

        modelBuilder.Entity<RouteLike>()
            .HasOne(e => e.Route)
            .WithMany(r => r.RouteLikes)
            .HasForeignKey(e => e.RouteId);

        return modelBuilder;
    }
}