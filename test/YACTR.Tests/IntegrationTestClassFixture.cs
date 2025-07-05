using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.IO.Converters;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using YACTR.Data;
using YACTR.Data.Model.Authentication;

namespace YACTR.Tests;

public class IntegrationTestClassFixture : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
{
    public TestWebApplicationFactory _factory { get; }
    public DatabaseContext _databaseContext;
    protected JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        Converters = { new GeoJsonConverterFactory() }
    };

    public IntegrationTestClassFixture(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _databaseContext = factory.Services.GetService<DatabaseContext>()!;
        _jsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
    }

    /// <summary>
    /// Ensures that the database is created before the test suite is run.
    /// </summary>
    /// <returns></returns>
    public async Task InitializeAsync()
    {
        await _databaseContext.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Truncates the database after the test suite is run.
    /// </summary>
    /// <returns></returns>
    public async Task DisposeAsync()
    {
        await TruncateTestDatabaseAsync();
    }

    /// <summary>
    /// Creates a test http client which automatically authenticates as the provided user when 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    protected HttpClient CreateAuthenticatedClient(User? user = null)
    {
        HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            TestAuthenticationHandler.AuthenticationScheme,
            TestAuthenticationHandler.GenerateAuthenticationToken(user ?? TestAuthenticationHandler.DEFAULT_TEST_USER));

        return client;
    }

    protected HttpClient CreateAnonymousClient()
    {
        return _factory.CreateClient();
    }

    protected async Task<T?> DeserializeEntityFromResponse<T>(HttpResponseMessage httpResponseMessage)
    {
        return JsonSerializer.Deserialize<T>(await httpResponseMessage.Content.ReadAsStringAsync(), _jsonSerializerOptions);
    }

    protected StringContent SerializeJsonFromRequestData<T>(T requestData)
    {
        return new StringContent(JsonSerializer.Serialize<T>(requestData, _jsonSerializerOptions),
            Encoding.UTF8, "application/json"); ;
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
        await _databaseContext.Database.ExecuteSqlRawAsync("""
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
            END $$;
        """, cancellationToken);
    }
}