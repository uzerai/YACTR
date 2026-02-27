using Microsoft.EntityFrameworkCore;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Organizations;

namespace YACTR.Infrastructure.Database.Table;

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

        modelBuilder.Entity<User>()
            .HasMany(e => e.RouteRatings)
            .WithOne(e => e.User);

        modelBuilder.Entity<User>()
            .HasMany(e => e.RouteLikes)
            .WithOne(e => e.User);

        modelBuilder.Entity<User>()
            .HasIndex(e => e.Email);
        
        modelBuilder.Entity<User>()
            .HasIndex(e => e.Auth0UserId);

        return modelBuilder;
    }
}