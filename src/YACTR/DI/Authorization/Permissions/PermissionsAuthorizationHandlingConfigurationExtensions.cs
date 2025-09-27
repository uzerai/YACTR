using Microsoft.AspNetCore.Authorization;
using YACTR.DI.Authorization.Permissions;

namespace YACTR.DI.Authorization.ConfigurationExtension;

public static class PermissionsAuthorizationHandlingConfigurationExtensions
{
    public static IServiceCollection AddPermissionsAuthorizationHandling(this IServiceCollection services)
    {
        services.AddTransient<IAuthorizationHandler, AdminPermissionsAuthorizationHandler>();
        services.AddTransient<IAuthorizationHandler, PlatformPermissionsAuthorizationHandler>();
        services.AddTransient<IAuthorizationHandler, OrganizationPermissionsAuthorizationHandler>();

        return services;
    }

}