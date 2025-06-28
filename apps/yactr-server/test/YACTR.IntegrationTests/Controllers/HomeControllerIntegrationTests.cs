using System.Net;
using System.Net.Http.Headers;

namespace YACTR.IntegrationTests.Controllers;

public class HomeControllerIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    public HomeControllerIntegrationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        
        // Setup authentication for protected endpoints
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyTestToken==");
    }
    
    [Fact]
    public async Task Index_WithValidAuthentication_ReturnsOk()
    {
        // Act
        var response = await _client.GetAsync("/");
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Index_WithoutAuthentication_ReturnsAuthorized()
    {
        // Arrange
        var client = _client;
        client.DefaultRequestHeaders.Authorization = null; // Remove authentication
        
        // Act
        var response = await client.GetAsync("/");
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
} 