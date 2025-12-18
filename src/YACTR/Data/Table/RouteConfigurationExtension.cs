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
            .WithOne(e => e.Route)
            .HasForeignKey(e => e.RouteId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Route>()
            .HasOne(e => e.TopoImage)
            .WithMany()
            .HasForeignKey(e => e.TopoImageId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Route>()
            .HasOne(e => e.SectorTopoImage)
            .WithMany()
            .HasForeignKey(e => e.SectorTopoImageId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Route>()
            .HasOne(e => e.SectorTopoImageOverlaySvg)
            .WithOne()
            .HasForeignKey<Route>(e => e.SectorTopoImageOverlaySvgId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Route>()
            .HasOne(e => e.TopoImageOverlaySvg)
            .WithOne()
            .HasForeignKey<Route>(e => e.TopoImageOverlaySvgId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Route>()
            .HasMany(e => e.RouteRatings)
            .WithOne(e => e.Route)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Route>()
            .HasMany(e => e.RouteLikes)
            .WithOne(e => e.Route)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure ClimbingType enum for Route - regular column (no computed column)
        modelBuilder.Entity<Route>()
            .Property(e => e.Type)
            .HasColumnType("climbing_type");

        modelBuilder.Entity<Route>()
            .Property(route => route.TopoLinePoints)
            .HasColumnType("jsonb");

        modelBuilder.Entity<Route>()
            .Property(route => route.SectorTopoLinePoints)
            .HasColumnType("jsonb");

        return modelBuilder;
    }
}