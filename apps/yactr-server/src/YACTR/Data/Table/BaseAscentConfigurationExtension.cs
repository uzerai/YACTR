
using YACTR.Model.Achievement;
using Microsoft.EntityFrameworkCore;

namespace YACTR.DI.Data.ConfigurationExtension;

public static class BaseAscentConfigurationExtension
{
    public static ModelBuilder ConfigureBaseAscentAbstractModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaseAscent>()
          .UseTphMappingStrategy()
          .ToTable("ascents")
          .HasDiscriminator<string>("ascent_type")
          .HasValue<RouteAscent>("route")
          .HasValue<PitchAscent>("pitch");
        
        return modelBuilder;
    }
}
