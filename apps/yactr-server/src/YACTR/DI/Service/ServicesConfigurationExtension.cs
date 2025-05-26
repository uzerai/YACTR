namespace YACTR.DI.Service;

public static class ServicesConfigurationExtension
{
    /// <summary>
    /// Adds all custom implementation services within the YACTR.DI.Service
    /// namespace to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>The service collection with the added services.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IImageStorageService, ImageStorageService>();

        return services;
    }
}
