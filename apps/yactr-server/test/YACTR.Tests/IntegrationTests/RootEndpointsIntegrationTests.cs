using System.Net;

namespace YACTR.Tests.Controllers;

[Collection("IntegrationTests")]
public class RootEndpointsIntegrationTests : IntegrationTestClassFixture
{
    public RootEndpointsIntegrationTests(TestWebApplicationFactory factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task Index_WithValidAuthentication_ReturnsOk()
    {
        using var client = CreateAuthenticatedClient();
        // Act
        var response = await client.GetAsync("/");
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task Index_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        using var client = CreateAnonymousClient();
        
        // Act
        var response = await client.GetAsync("/");
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
} 