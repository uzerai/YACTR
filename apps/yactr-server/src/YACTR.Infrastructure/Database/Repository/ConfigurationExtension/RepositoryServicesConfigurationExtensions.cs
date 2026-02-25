using Microsoft.Extensions.DependencyInjection;
using YACTR.Domain.Model;
using YACTR.Domain.Model.Achievement;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Climbing;
using YACTR.Domain.Model.Climbing.Rating;
using YACTR.Domain.Model.Organizations;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Infrastructure.Database.Repository.ConfigurationExtension;

public static class RepositoryServicesConfigurationExtensions
{
    /// <summary>
    /// Registers project repository services.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IEntityRepository<User>, EntityRepository<User>>();
        services.AddTransient<IEntityRepository<Organization>, EntityRepository<Organization>>();
        // IRepository as it's a pivot table, and doesn't need created/updated/deleted at.
        services.AddTransient<IRepository<OrganizationUser>, BaseRepository<OrganizationUser>>();
        services.AddTransient<IEntityRepository<OrganizationTeam>, EntityRepository<OrganizationTeam>>();
        services.AddTransient<IRepository<OrganizationTeamUser>, BaseRepository<OrganizationTeamUser>>();
        services.AddTransient<IEntityRepository<Area>, EntityRepository<Area>>();
        services.AddTransient<IEntityRepository<Sector>, EntityRepository<Sector>>();
        services.AddTransient<IEntityRepository<Pitch>, EntityRepository<Pitch>>();
        services.AddTransient<IEntityRepository<Route>, EntityRepository<Route>>();
        services.AddTransient<IEntityRepository<Image>, EntityRepository<Image>>();
        services.AddTransient<IEntityRepository<RouteRating>, EntityRepository<RouteRating>>();
        services.AddTransient<IEntityRepository<RouteLike>, EntityRepository<RouteLike>>();

        // Ascent repository - using IRepository since Ascent doesn't inherit from BaseEntity
        services.AddTransient<IRepository<Ascent>, BaseRepository<Ascent>>();

        return services;
    }
}
