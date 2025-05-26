using Microsoft.EntityFrameworkCore;
using YACTR.Model.Organizations;

namespace YACTR.DI.Data.ConfigurationExtension;

public static class OrganizationTeamConfigurationExtension
{
    public static ModelBuilder ConfigureOrganizationTeamModel(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OrganizationTeam>()
            .HasOne(e => e.Organization)
            .WithMany(e => e.Teams)
            .HasForeignKey(e => e.OrganizationId);

        modelBuilder.Entity<OrganizationTeam>()
            .HasMany(e => e.OrganizationTeamUsers)
            .WithOne(e => e.OrganizationTeam);

        modelBuilder.Entity<OrganizationTeam>()
            .HasMany(e => e.Users)
            .WithMany()
            .UsingEntity<OrganizationTeamUser>();

        return modelBuilder;
    }
}