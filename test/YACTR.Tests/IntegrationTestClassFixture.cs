using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Bogus;
using FastEndpoints.Testing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetTopologySuite.IO.Converters;
using NodaTime;
using YACTR.Data;
using YACTR.Data.Model;
using YACTR.Data.Model.Authentication;
using YACTR.Data.QueryExtensions;
using YACTR.Data.Repository.ConfigurationExtension;
using YACTR.Data.Repository.Interface;

namespace YACTR.Tests;

public class IntegrationTestClassFixture : AppFixture<Program>
{
    public DatabaseContext DatabaseContext { get; private set; } = null!;
    public HttpClient AnonymousClient { get; private set; } = null!;
    protected JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        Converters = { new GeoJsonConverterFactory() }
    };

    /// <summary>
    /// Ensures that the database is created before the test suite is run.
    /// </summary>
    /// <returns></returns>
    protected override async ValueTask SetupAsync()
    {
        AnonymousClient = CreateClient();
        DatabaseContext = Services.GetRequiredService<DatabaseContext>();
        await DatabaseContext.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Truncates the database after the test suite is run.
    /// </summary>
    /// <returns></returns>
    protected override async ValueTask TearDownAsync()
    {
        await TruncateTestDatabaseAsync();
    }

    protected override void ConfigureApp(IWebHostBuilder builder)
    {
        base.ConfigureApp(builder);

        builder.UseEnvironment("Test");
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        services.AddControllers();
        services.AddRouting();

        services.Configure<DbContextOptionsBuilder>(options =>
        {
            options.UseNpgsql("Host=localhost;Database=yactr_test;Username=yactr;Password=yactr;Port=5432");
            // options.EnableDetailedErrors();
        });

        // Remove the main authentication scheme from main application;
        // replace with the TestAuthenticationHandler which receives a magic string as Bearer token
        // and authorizes as the user references in the magic string.
        services.RemoveAll<AuthenticationSchemeOptions>();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = TestAuthenticationHandler.AuthenticationScheme;
            options.DefaultChallengeScheme = TestAuthenticationHandler.AuthenticationScheme;
        }).AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
            TestAuthenticationHandler.AuthenticationScheme,
            options => { });

        services.AddRepositories();
        services.AddSingleton<IClock>(NodaTime.SystemClock.Instance);
    }

    /// <summary>
    /// Creates a test http client which is authenticated against the provided user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public HttpClient CreateAuthenticatedClient(User? user = null)
    {
        HttpClient client = CreateClient();

        user ??= TestAuthenticationHandler.DEFAULT_TEST_USER;

        // If the user has not been created; create it and assign the user to created one.
        if (!DatabaseContext.Users.WhereAuth0UserId(user.Auth0UserId).Any())
        {
            user = DatabaseContext.Users.Add(user).Entity;
            DatabaseContext.SaveChanges();
        }

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            TestAuthenticationHandler.AuthenticationScheme,
            TestAuthenticationHandler.GenerateAuthenticationToken(user));

        return client;
    }

    public IEntityRepository<T> GetEntityRepository<T>() where T : BaseEntity
    {
        return Services.GetRequiredService<IEntityRepository<T>>();
    }

    public IRepository<T> GetRepository<T>() where T : class
    {
        return Services.GetRequiredService<IRepository<T>>();
    }

    /// <summary>
    /// Currently the test suite is ran against a single database which is on the host computer.
    /// This method truncates the database of the current database context to ensure that the test suite
    /// can run with similar information without having to create a new database for each test.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task TruncateTestDatabaseAsync(CancellationToken cancellationToken = default)
    {
        await DatabaseContext.Database.ExecuteSqlRawAsync("""
            DO $$
            DECLARE
                r RECORD;
            BEGIN
                -- Disable foreign key constraints temporarily
                SET session_replication_role = replica;
                
                -- Loop through all tables in the public schema and truncate them
                FOR r IN (SELECT tablename FROM pg_tables WHERE schemaname = 'public') LOOP
                    EXECUTE 'TRUNCATE TABLE ' || quote_ident(r.tablename) || ' CASCADE';
                END LOOP;
                
                -- Re-enable foreign key constraints
                SET session_replication_role = DEFAULT;
                
                RAISE NOTICE 'All tables in public schema have been truncated successfully.';
            END $$
        """, cancellationToken);
    }
}