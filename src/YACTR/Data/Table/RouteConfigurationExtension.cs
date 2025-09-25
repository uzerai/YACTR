
using YACTR.Data.Model.Climbing;
using Microsoft.EntityFrameworkCore;
using Route = YACTR.Data.Model.Climbing.Route;

namespace YACTR.Data.ConfigurationExtension;

public static class RouteConfigurationExtension
{
    public static ModelBuilder ConfigureRouteModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Route>()
            .HasOne(e => e.Sector)
            .WithMany(e => e.Routes)
            .HasForeignKey(e => e.SectorId);

        modelBuilder.Entity<Route>()
            .HasMany(e => e.Pitches)
            .WithMany(e => e.Routes)
            .UsingEntity<RoutePitch>();

        modelBuilder.Entity<Route>()
            .HasOne(e => e.TopoImage)
            .WithOne(e => e.RelatedEntity as Route)
            .HasForeignKey<Route>(e => e.TopoImageId);

        modelBuilder.Entity<Route>()
            .HasMany(e => e.RouteRatings)
            .WithOne(e => e.Route);

        modelBuilder.Entity<Route>()
            .HasMany(e => e.RouteLikes)
            .WithOne(e => e.Route);

        // Configure ClimbingType enum for Route - regular column (no computed column)
        modelBuilder.Entity<Route>()
            .Property(e => e.Type)
            .HasColumnType("climbing_type");

        return modelBuilder;
    }
}