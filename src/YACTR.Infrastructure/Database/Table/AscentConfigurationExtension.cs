using Microsoft.EntityFrameworkCore;
using YACTR.Domain.Model.Achievement;

namespace YACTR.Infrastructure.Database.Table;

public static class AscentConfigurationExtension
{
    public static ModelBuilder ConfigureAscentModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ascent>()
            .ToTable("ascents");

        modelBuilder.Entity<Ascent>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId);

        modelBuilder.Entity<Ascent>()
            .HasOne(e => e.Route)
            .WithMany()
            .HasForeignKey(e => e.RouteId);

        return modelBuilder;
    }
}
