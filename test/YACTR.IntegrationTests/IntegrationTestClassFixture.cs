using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.IO.Converters;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using YACTR.Data;
using YACTR.Data.Model.Authentication;

namespace YACTR.IntegrationTests;

public class IntegrationTestClassFixture : IClassFixture<TestWebApplicationFactory>
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
}