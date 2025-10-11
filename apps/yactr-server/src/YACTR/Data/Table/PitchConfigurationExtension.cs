using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Climbing;

namespace YACTR.Data.ConfigurationExtension;

public static class PitchConfigurationExtension
{
    public static ModelBuilder ConfigurePitchModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pitch>()
            .HasOne(e => e.Sector)
            .WithMany()
            .HasForeignKey(e => e.SectorId);

        modelBuilder.Entity<Pitch>()
            .HasOne(e => e.RouteSvgOverlay)
            .WithOne()
            .HasForeignKey<Pitch>(e => e.RouteSvgOverlayId);

        // Configure ClimbingType enum for Pitch - will use PostgreSQL enum
        modelBuilder.Entity<Pitch>()
            .Property(e => e.Type)
            .HasColumnType("climbing_type");

        return modelBuilder;
    }
}