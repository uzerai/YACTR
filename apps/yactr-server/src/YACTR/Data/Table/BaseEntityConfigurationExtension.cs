using Microsoft.EntityFrameworkCore;
using YACTR.Model;

namespace YACTR.DI.Data.ConfigurationExtension;

public static class BaseEntityConfigurationExtension
{
    public static ModelBuilder ConfigureBaseEntityAbstractModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BaseEntity>()
          .UseTpcMappingStrategy();
        
        return modelBuilder;
    }
}
