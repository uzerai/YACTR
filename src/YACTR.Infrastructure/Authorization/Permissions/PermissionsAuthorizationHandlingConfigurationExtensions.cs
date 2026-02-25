using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace YACTR.Infrastructure.Authorization.Permissions;

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