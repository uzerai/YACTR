using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.DI.Data.ConfigurationExtension;
using YACTR.Model;
using YACTR.Model.Authentication;
using YACTR.Model.Authorization.Permissions;
using YACTR.Model.Organizations;
using Route = YACTR.Model.Location.Route;
using YACTR.Model.Location;
using YACTR.Model.Achievement;

namespace YACTR.DI.Data;

public class DatabaseContext : DbContext
{
    private readonly IClock _clock;

    public DatabaseContext(DbContextOptions<DatabaseContext> options, IClock clock)
        : base(options)
    {
        _clock = clock;
    }

    // ############################################################
    // ##########  ENTITY SETS   ##################################
    // ############################################################
    public DbSet<User> Users { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<OrganizationUser> OrganizationUsers { get; set; }
    public DbSet<OrganizationPermission> OrganizationPermissions { get; set; }
    public DbSet<OrganizationTeam> OrganizationTeams { get; set; }
    public DbSet<OrganizationTeamUser> OrganizationTeamUsers { get; set; }
    public DbSet<Area> Crags { get; set; }
    public DbSet<Sector> Sectors { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<Pitch> Pitches { get; set; }
    public DbSet<RoutePitch> RoutePitches { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<RouteAscent> RouteAscents { get; set; }
    public DbSet<PitchAscent> PitchAscents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // For inherited abstract models.
        modelBuilder.ConfigureBaseEntityAbstractModel();

        // Regular model configurations.
        modelBuilder.ConfigureUserModel();
        modelBuilder.ConfigureOrganizationModel();
        modelBuilder.ConfigureOrganizationUserModel();
        modelBuilder.ConfigureOrganizationPermissionModel();
        modelBuilder.ConfigureOrganizationTeamModel();
        modelBuilder.ConfigureOrganizationTeamUserModel();
        modelBuilder.ConfigureAreaModel();
        modelBuilder.ConfigureSectorModel();
        modelBuilder.ConfigureRouteModel();
        modelBuilder.ConfigurePitchModel();
        modelBuilder.ConfigureRoutePitchModel();
        modelBuilder.ConfigureImageModel();
        modelBuilder.ConfigureBaseAscentAbstractModel();
    }

    /// <summary>
    /// This override of SaveChangesAsync() is used to set the CreatedAt and UpdatedAt properties
    /// for all entities inheriting from BaseEntity that are being saved.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>int</returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        SetEntityTimestamps();

        return base.SaveChangesAsync();
    }

    /// <summary>
    /// Since the BaseEntity is the defacto parent of all database entities in the app;
    /// we want to keep track of the timestamps related to each on every save operation.
    /// 
    /// This method achieves that by checking the state of each entry in the ChangeTracker:
    /// if _and only if_ the entry is an instance of BaseEntity, will it set the CreatedAt and UpdatedAt
    /// properties to the current date and time.
    /// 
    /// This allows unmanaged (non-entity) objects to be saved normally in addition to our custom handling
    /// for our own BaseEntity objects.
    /// </summary>
    private void SetEntityTimestamps()
    {
        var entries = ChangeTracker.Entries();

        foreach (var entry in entries)
        {
            if (entry.Entity is BaseEntity entity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.CreatedAt = _clock.GetCurrentInstant();
                        entity.UpdatedAt = _clock.GetCurrentInstant();
                        break;
                    case EntityState.Modified:
                        entity.UpdatedAt = _clock.GetCurrentInstant();
                        break;
                }
            }
        }
    }
}