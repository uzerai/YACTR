using Microsoft.EntityFrameworkCore;

using YACTR.Domain.Model;

namespace YACTR.Infrastructure.Database.Table;

public static class CountryDataConfigurationExtension
{
    public static ModelBuilder ConfigureCountryDataModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CountryData>(entity =>
        {
            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.CountryName)
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnName("country_name");

            entity.Property(e => e.AdminName)
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnName("admin_name");

            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(16)
                .HasColumnName("code");

            entity.Property(e => e.Continent)
                .IsRequired()
                .HasMaxLength(64)
                .HasColumnName("continent");

            entity.Property(e => e.Region)
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnName("region");

            entity.Property(e => e.Subregion)
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnName("subregion");

            entity.Property(e => e.WorldBlock)
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnName("world_block");

            entity.Property(e => e.Geometry)
                .IsRequired()
                .HasColumnName("geometry");
        });

        return modelBuilder;
    }
}