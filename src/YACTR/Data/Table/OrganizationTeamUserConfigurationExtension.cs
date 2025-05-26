using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Organizations;

namespace YACTR.Data.ConfigurationExtension;

public static class OrganizationTeamUserConfigurationExtension
{
    public static ModelBuilder ConfigureOrganizationTeamUserModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrganizationTeamUser>()
            .HasOne(e => e.OrganizationTeam)
            .WithMany(e => e.OrganizationTeamUsers)
            .HasForeignKey(e => e.OrganizationTeamId);

        modelBuilder.Entity<OrganizationTeamUser>()
            .HasOne(e => e.OrganizationUser)
            .WithMany(e => e.OrganizationTeamUsers)
            .HasForeignKey(e => new { e.OrganizationId, e.UserId });

        modelBuilder.Entity<OrganizationTeamUser>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId);

        modelBuilder.Entity<OrganizationTeamUser>()
            .HasOne(e => e.Organization)
            .WithMany()
            .HasForeignKey(e => e.OrganizationId);

        modelBuilder.Entity<OrganizationTeamUser>()
            .HasKey(e => new { e.OrganizationId, e.OrganizationTeamId, e.UserId });

        return modelBuilder;
    }
}
