using Microsoft.EntityFrameworkCore;
using YACTR.Model.Authorization.Permissions;

namespace YACTR.DI.Data.ConfigurationExtension;

public static class OrganizationPermissionConfigurationExtension
{
    public static ModelBuilder ConfigureOrganizationPermissionModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrganizationPermission>()
            .HasKey(e => new { e.OrganizationId, e.UserId, e.Permission });

        modelBuilder.Entity<OrganizationPermission>()
            .HasOne(e => e.OrganizationUser)
            .WithMany(e => e.Permissions)
            .HasForeignKey(e => new { e.OrganizationId, e.UserId });

        modelBuilder.Entity<OrganizationPermission>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId);

        modelBuilder.Entity<OrganizationPermission>()
            .HasOne(e => e.Organization)
            .WithMany()
            .HasForeignKey(e => e.OrganizationId);

        return modelBuilder;
    }
}