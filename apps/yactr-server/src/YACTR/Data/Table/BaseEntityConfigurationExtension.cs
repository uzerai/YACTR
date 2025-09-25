using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model;

namespace YACTR.Data.ConfigurationExtension;

public static class BaseEntityConfigurationExtension
{
    public static ModelBuilder ConfigureBaseEntityAbstractModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaseEntity>()
          .UseTpcMappingStrategy();

        return modelBuilder;
    }
}
