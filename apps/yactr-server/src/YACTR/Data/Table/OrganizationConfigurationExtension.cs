using Microsoft.EntityFrameworkCore;
using YACTR.Model.Organizations;

namespace YACTR.DI.Data.ConfigurationExtension;

public static class OrganizationConfigurationExtension
{
    public static ModelBuilder ConfigureOrganizationModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Organization>()
            .HasMany(e => e.OrganizationUsers)
            .WithOne(e => e.Organization);
        modelBuilder.Entity<Organization>()
            .HasMany(e => e.Users)
            .WithMany(e => e.Organizations)
            .UsingEntity<OrganizationUser>();
        modelBuilder.Entity<Organization>()
            .HasMany(e => e.Teams)
            .WithOne(e => e.Organization);

        return modelBuilder;
    }
}