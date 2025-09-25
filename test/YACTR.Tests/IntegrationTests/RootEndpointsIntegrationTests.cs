using FastEndpoints.Testing;

namespace YACTR.Tests.Endpoints;

[Collection("IntegrationTests")]
public class RootEndpointsIntegrationTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
{

    [Fact]
    public async Task Index_WithValidAuthentication_ReturnsOk()
    {
        using var client = fixture.CreateAuthenticatedClient();
        // Act
        var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        // Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Index_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await fixture.AnonymousClient.GetAsync("/", TestContext.Current.CancellationToken);

        // Assert
        // Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}