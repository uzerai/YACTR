using Microsoft.AspNetCore.Authorization;
using YACTR.DI.Authorization.Permissions;

namespace YACTR.DI.Authorization.ConfigurationExtension;

public static class PermissionsAuthorizationHandlingConfigurationExtensions
{
    public static IServiceCollection AddPermissionsAuthorizationHandling(this IServiceCollection services)
    {
        services.AddTransient<IAuthorizationHandler, OrganizationPermissionsAuthorizationHandler>();
        services.AddTransient<IAuthorizationHandler, PlatformPermissionsAuthorizationHandler>();

        return services;
    }

}