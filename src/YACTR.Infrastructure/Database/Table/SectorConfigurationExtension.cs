using Microsoft.EntityFrameworkCore;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Infrastructure.Database.Table;

public static class SectorConfigurationExtension
{
    public static ModelBuilder ConfigureSectorModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sector>()
            .HasOne(e => e.Area)
            .WithMany(e => e.Sectors)
            .HasForeignKey(e => e.AreaId);

        modelBuilder.Entity<Sector>()
            .HasMany(e => e.Routes)
            .WithOne(e => e.Sector);

        modelBuilder.Entity<Sector>()
            .HasOne(e => e.PrimarySectorImage)
            .WithOne()
            .HasForeignKey<Sector>(e => e.PrimarySectorImageId);

        modelBuilder.Entity<Sector>()
            .HasMany(e => e.SectorImages)
            .WithOne(e => e.Sector)
            .HasForeignKey(e => e.SectorId);

        return modelBuilder;
    }
}