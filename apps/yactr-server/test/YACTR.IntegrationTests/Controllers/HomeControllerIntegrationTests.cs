using System.Net;
using System.Net.Http.Headers;

namespace YACTR.IntegrationTests.Controllers;

public class HomeControllerIntegrationTests : IntegrationTestClassFixture
{
    public HomeControllerIntegrationTests(TestWebApplicationFactory factory) : base(factory)
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
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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