using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NodaTime;
using YACTR.Data;
using YACTR.Data.Repository.ConfigurationExtension;

namespace YACTR.IntegrationTests;

/// <summary>
/// A test factory for creating a test application context.
/// This context connects to a local database instance (see docker-compose.yml).
/// 
/// Note that as part of the configuration, the database is dropped and recreated _BEFORE_ starting each test.
/// This should allow for inspection of the database state when a single test has ran.
/// TODO: It does however mean that running the tests in parallel is not possible as the database will need to be recreated.
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Call base first
        base.ConfigureWebHost(builder);
        builder.UseEnvironment("Test");
        
        builder.ConfigureServices(services =>
        {
            // Configure services similar to your main application
            services.AddControllers();
            services.AddRouting();

            services.Configure<DbContextOptionsBuilder>(options => {
                options.UseNpgsql("Host=localhost;Database=yactr_test;Username=yactr;Password=yactr;Port=5432");
            });
            
            services.RemoveAll<AuthenticationSchemeOptions>();
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = TestAuthenticationHandler.AuthenticationScheme;
                options.DefaultChallengeScheme = TestAuthenticationHandler.AuthenticationScheme;
            }).AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                TestAuthenticationHandler.AuthenticationScheme,
                options => {});

            RegisterServices(services);
        });
    }

    protected void RegisterServices(IServiceCollection services) {
        //TODO: Make this available to configure/override as part of the test suite settings.
        services.AddSingleton<IClock>(NodaTime.SystemClock.Instance);
        services.AddRepositories();
    }
}
