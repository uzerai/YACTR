using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Organizations;

namespace YACTR.Data.ConfigurationExtension;

public static class OrganizationUserConfigurationExtension
{
    public static ModelBuilder ConfigureOrganizationUserModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrganizationUser>()
              .HasMany(e => e.Permissions)
              .WithOne(e => e.OrganizationUser);
        modelBuilder.Entity<OrganizationUser>()
            .HasKey(e => new { e.OrganizationId, e.UserId });
        modelBuilder.Entity<OrganizationUser>()
            .HasOne(e => e.Organization)
            .WithMany(e => e.OrganizationUsers)
            .HasForeignKey(e => e.OrganizationId);
        modelBuilder.Entity<OrganizationUser>()
            .HasOne(e => e.User)
            .WithMany(e => e.OrganizationUsers)
            .HasForeignKey(e => e.UserId);

        return modelBuilder;
    }

}