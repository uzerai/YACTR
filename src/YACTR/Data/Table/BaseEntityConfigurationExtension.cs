using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model;

namespace YACTR.Data.ConfigurationExtension;

public static class BaseEntityConfigurationExtension
{
    public static ModelBuilder ConfigureBaseEntityAbstractModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaseEntity>()
          .UseTpcMappingStrategy();

        modelBuilder.Entity<BaseEntity>()
            .HasIndex(e => e.CreatedAt)
            .HasMethod("brin")
            .IsDescending();

        modelBuilder.Entity<BaseEntity>()
            .HasIndex(e => e.UpdatedAt)
            .HasMethod("brin")
            .IsDescending();

        return modelBuilder;
    }
}
