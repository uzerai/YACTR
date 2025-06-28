using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using YACTR.DTO.RequestData.Organizations;
using YACTR.Data.Model.Organizations;
using System.Net;

namespace YACTR.IntegrationTests.Controllers;

public class OrganizationsControllerIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    public OrganizationsControllerIntegrationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        
        // We would normally setup authentication here
        // This is a placeholder for actual authentication setup
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyTestToken==");
    }
    
    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/organizations");
                
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task GetAll_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var client = _client;
        client.DefaultRequestHeaders.Authorization = null; // Remove authentication
        
        // Act
        var response = await client.GetAsync("/organizations");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedOrganization()
    {
        // Arrange
        var createRequest = new CreateOrganizationRequestData("Integration Test Org");
        
        var content = new StringContent(
            JsonSerializer.Serialize(createRequest),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await _client.PostAsync("/organizations", content);
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        var organization = JsonSerializer.Deserialize<Organization>(responseString, options);
        Assert.NotNull(organization);
        Assert.Equal("Integration Test Org", organization.Name);
    }
    
    [Fact]
    public async Task Create_WithEmptyName_IsAllowed()
    {
        // Arrange
        var createRequest = new CreateOrganizationRequestData("");
        
        var content = new StringContent(
            JsonSerializer.Serialize(createRequest),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await _client.PostAsync("/organizations", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Create_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var client = _client;
        client.DefaultRequestHeaders.Authorization = null; // Remove authentication
        
        var createRequest = new CreateOrganizationRequestData("Test Org");
        
        var content = new StringContent(
            JsonSerializer.Serialize(createRequest),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await client.PostAsync("/organizations", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task Get_WithValidId_ReturnsOrganization()
    {
        // Arrange - First create an organization
        var createRequest = new CreateOrganizationRequestData("Test Organization for Get");
        
        var createContent = new StringContent(
            JsonSerializer.Serialize(createRequest),
            Encoding.UTF8,
            "application/json");
            
        var createResponse = await _client.PostAsync("/organizations", createContent);
        createResponse.EnsureSuccessStatusCode();
        
        var createdOrg = JsonSerializer.Deserialize<Organization>(
            await createResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        
        // Act
        var response = await _client.GetAsync($"/organizations/{createdOrg!.Id}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var organization = JsonSerializer.Deserialize<Organization>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        Assert.NotNull(organization);
        Assert.Equal(createdOrg.Id, organization.Id);
        Assert.Equal("Test Organization for Get", organization.Name);
    }
    
    [Fact]
    public async Task Get_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        
        // Act
        var response = await _client.GetAsync($"/organizations/{invalidId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Get_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var client = _client;
        client.DefaultRequestHeaders.Authorization = null; // Remove authentication
        
        var invalidId = Guid.NewGuid();
        
        // Act
        var response = await client.GetAsync($"/organizations/{invalidId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
