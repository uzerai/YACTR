using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Data.ConfigurationExtension;
using YACTR.Data.Model;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Model.Authentication;
using YACTR.Data.Model.Climbing;
using YACTR.Data.Model.Climbing.Rating;
using YACTR.Data.Model.Organizations;
using Route = YACTR.Data.Model.Climbing.Route;

namespace YACTR.Data;

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
    public DbSet<OrganizationTeam> OrganizationTeams { get; set; }
    public DbSet<OrganizationTeamUser> OrganizationTeamUsers { get; set; }
    public DbSet<Area> Areas { get; set; }
    public DbSet<Sector> Sectors { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<Pitch> Pitches { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Ascent> Ascents { get; set; }
    public DbSet<RouteRating> RouteRatings { get; set; }
    public DbSet<RouteLike> RouteLikes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure PostgreSQL enum
        modelBuilder.HasPostgresEnum<ClimbingType>();

        // For inherited abstract models.
        modelBuilder.ConfigureBaseEntityAbstractModel();

        // Regular model configurations.
        modelBuilder.ConfigureUserModel();
        modelBuilder.ConfigureOrganizationModel();
        modelBuilder.ConfigureOrganizationUserModel();
        modelBuilder.ConfigureOrganizationTeamModel();
        modelBuilder.ConfigureOrganizationTeamUserModel();

        modelBuilder.ConfigureAreaModel();
        modelBuilder.ConfigureSectorModel();
        modelBuilder.ConfigureRouteModel();
        modelBuilder.ConfigurePitchModel();
        modelBuilder.ConfigureRouteRatingModel();
        modelBuilder.ConfigureRouteLikeModel();

        modelBuilder.ConfigureImageModel();

        modelBuilder.ConfigureAscentModel();
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