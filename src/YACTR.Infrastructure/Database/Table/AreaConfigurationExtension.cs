using Microsoft.EntityFrameworkCore;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Infrastructure.Database.Table;

public static class AreaConfigurationExtension
{
    public static ModelBuilder ConfigureAreaModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Area>()
            .ToTable("areas");

        modelBuilder.Entity<Area>()
            .HasOne(e => e.MaintainerOrganization)
            .WithMany()
            .HasForeignKey(e => e.MaintainerOrganizationId);

        modelBuilder.Entity<Area>()
            .HasMany(e => e.Sectors)
            .WithOne(e => e.Area);

        return modelBuilder;
    }
}