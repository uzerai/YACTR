using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using YACTR.DTO.RequestData.Organizations;
using YACTR.Data.Model.Organizations;
using System.Net;

namespace YACTR.IntegrationTests.Controllers;

public class OrganizationTeamsControllerIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    public OrganizationTeamsControllerIntegrationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        
        // Setup authentication for protected endpoints
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyTestToken==");
    }
    
    [Fact]
    public async Task GetAll_WithValidOrganizationId_ReturnsSuccessStatusCode()
    {
        // Arrange - First create an organization
        var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Teams");
        
        var orgContent = new StringContent(
            JsonSerializer.Serialize(createOrgRequest),
            Encoding.UTF8,
            "application/json");
            
        var orgResponse = await _client.PostAsync("/organizations", orgContent);
        orgResponse.EnsureSuccessStatusCode();
        
        var organization = JsonSerializer.Deserialize<Organization>(
            await orgResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        
        // Act
        var response = await _client.GetAsync($"/organizations/{organization!.Id}/teams");
                
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task GetAll_WithInvalidOrganizationId_ReturnsNotFound()
    {
        // Arrange
        var invalidOrgId = Guid.NewGuid();
        
        // Act
        var response = await _client.GetAsync($"/organizations/{invalidOrgId}/teams");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedTeam()
    {
        // Arrange - First create an organization
        var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Team Creation");
        
        var orgContent = new StringContent(
            JsonSerializer.Serialize(createOrgRequest),
            Encoding.UTF8,
            "application/json");
            
        var orgResponse = await _client.PostAsync("/organizations", orgContent);
        orgResponse.EnsureSuccessStatusCode();
        
        var organization = JsonSerializer.Deserialize<Organization>(
            await orgResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        
        // Create team request
        var createRequest = new CreateOrganizationTeamRequestData("Test Team");
        
        var content = new StringContent(
            JsonSerializer.Serialize(createRequest),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await _client.PostAsync($"/organizations/{organization!.Id}/teams", content);
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        var team = JsonSerializer.Deserialize<OrganizationTeam>(responseString, options);
        Assert.NotNull(team);
        Assert.Equal("Test Team", team.Name);
        Assert.Equal(organization.Id, team.OrganizationId);
    }
    
    [Fact]
    public async Task Create_WithInvalidOrganizationId_ReturnsNotFound()
    {
        // Arrange
        var invalidOrgId = Guid.NewGuid();
        var createRequest = new CreateOrganizationTeamRequestData("Test Team");
        
        var content = new StringContent(
            JsonSerializer.Serialize(createRequest),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await _client.PostAsync($"/organizations/{invalidOrgId}/teams", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Create_WithEmptyTeamName_ReturnsBadRequest()
    {
        // Arrange - First create an organization
        var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Empty Team");
        
        var orgContent = new StringContent(
            JsonSerializer.Serialize(createOrgRequest),
            Encoding.UTF8,
            "application/json");
            
        var orgResponse = await _client.PostAsync("/organizations", orgContent);
        orgResponse.EnsureSuccessStatusCode();
        
        var organization = JsonSerializer.Deserialize<Organization>(
            await orgResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        
        var createRequest = new CreateOrganizationTeamRequestData("");
        
        var content = new StringContent(
            JsonSerializer.Serialize(createRequest),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await _client.PostAsync($"/organizations/{organization!.Id}/teams", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
} 