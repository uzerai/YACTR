
using YACTR.Data.Model.Achievement;
using Microsoft.EntityFrameworkCore;

namespace YACTR.Data.ConfigurationExtension;

public static class BaseAscentConfigurationExtension
{
    public static ModelBuilder ConfigureAscentModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ascent>()
            .ToTable("ascents");
        
        return modelBuilder;
    }
}
