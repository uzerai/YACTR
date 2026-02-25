using Microsoft.EntityFrameworkCore;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Infrastructure.Database.Table;

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

        modelBuilder.Entity<Pitch>()
            .Property(e => e.Type)
            .HasColumnType("climbing_type");

        return modelBuilder;
    }
}