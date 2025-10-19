using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model;

public static class SectorImageConfigurationExtension
{
    public static ModelBuilder ConfigureSectorImageModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SectorImage>()
            .ToTable("sector_images");

        modelBuilder.Entity<SectorImage>()
            .HasKey(e => new { e.SectorId, e.ImageId });

        modelBuilder.Entity<SectorImage>()
            .HasOne(e => e.Sector)
            .WithMany(e => e.SectorImages)
            .HasForeignKey(e => e.SectorId);

        modelBuilder.Entity<SectorImage>()
            .HasOne(e => e.Image)
            .WithMany()
            .HasForeignKey(e => e.ImageId);

        return modelBuilder;
    }
}