using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using YACTR.DTO.RequestData.Organizations;
using YACTR.Data.Model.Organizations;

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
}
