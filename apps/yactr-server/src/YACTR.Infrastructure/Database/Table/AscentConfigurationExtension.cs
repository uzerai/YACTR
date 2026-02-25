using Microsoft.EntityFrameworkCore;
using YACTR.Domain.Model.Achievement;

namespace YACTR.Infrastructure.Database.Table;

public static class AscentConfigurationExtension
{
    public static ModelBuilder ConfigureAscentModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ascent>()
            .ToTable("ascents");

        return modelBuilder;
    }
}
