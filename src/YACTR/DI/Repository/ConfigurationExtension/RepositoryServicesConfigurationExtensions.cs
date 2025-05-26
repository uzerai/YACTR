using YACTR.DI.Repository.Interface;
using YACTR.Model;
using Route = YACTR.Model.Location.Route;

namespace YACTR.DI.Repository.ConfigurationExtension;

public static class RepositoryServicesConfigurationExtensions
{
    /// <summary>
    /// Registers project repository services.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IOrganizationRepository, OrganizationRepository>();
        services.AddTransient<IOrganizationUserRepository, OrganizationUserRepository>();
        services.AddTransient<IOrganizationTeamRepository, OrganizationTeamRepository>();
        services.AddTransient<IAreaRepository, AreaRepository>();
        services.AddTransient<ISectorRepository, SectorRepository>();
        services.AddTransient<IPitchRepository, PitchRepository>();
        services.AddTransient<IEntityRepository<Route>, RouteRepository>();
        services.AddTransient<IEntityRepository<Image>, ImageRepository>();
        
        return services;
    }
}
