using System.Net;
using FastEndpoints.Testing;
using Shouldly;

namespace YACTR.Tests.EndpointTests;

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
    }

    [Fact]
    public async Task Index_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await fixture.CreateClient().GetAsync("/", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}