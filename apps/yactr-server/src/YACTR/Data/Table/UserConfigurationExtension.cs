using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Authentication;
using YACTR.Data.Model.Organizations;

namespace YACTR.Data.ConfigurationExtension;

public static class UserConfigurationExtension
{
    public static ModelBuilder ConfigureUserModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(e => e.Organizations)
            .WithMany(e => e.Users)
            .UsingEntity<OrganizationUser>();
        modelBuilder.Entity<User>()
            .HasMany(e => e.OrganizationUsers)
            .WithOne(e => e.User);

        return modelBuilder;
    }
}