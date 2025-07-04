using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using YACTR.DTO.RequestData.Organizations;
using YACTR.Data.Model.Organizations;
using System.Net;

namespace YACTR.IntegrationTests.Controllers;

public class OrganizationTeamsControllerIntegrationTests :  IntegrationTestClassFixture
{
    public OrganizationTeamsControllerIntegrationTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAll_WithValidOrganizationId_ReturnsSuccessStatusCode()
    {
        var client = CreateAuthenticatedClient();
        // Arrange - First create an organization
        var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Teams");
        
        var orgContent = new StringContent(
            JsonSerializer.Serialize(createOrgRequest, _jsonSerializerOptions),
            Encoding.UTF8,
            "application/json");

        var orgResponse = await client.PostAsync("/organizations", orgContent);
        orgResponse.EnsureSuccessStatusCode();
        
        var organization = JsonSerializer.Deserialize<Organization>(
            await orgResponse.Content.ReadAsStringAsync(),
            _jsonSerializerOptions
        );
        
        // Act
        var response = await client.GetAsync($"/organizations/{organization!.Id}/teams");
                
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    /// <summary>
    /// In this case; a user cannot have permissions for an organization which does not exist;
    /// therefore, forbidden is returned instead of not found.
    /// 
    /// It's also in the interest of enumeration attacks in the quantum computing age :')
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetAll_WithInvalidOrganizationId_ReturnsForbidden()
    {
        var client = CreateAuthenticatedClient();
        // Arrange
        var invalidOrgId = Guid.NewGuid();
        
        // Act
        var response = await client.GetAsync($"/organizations/{invalidOrgId}/teams");
        
        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedTeam()
    {
        var client = CreateAuthenticatedClient();
        // Arrange - First create an organization
        var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Team Creation");
        
        var orgContent = new StringContent(
            JsonSerializer.Serialize(createOrgRequest),
            Encoding.UTF8,
            "application/json");
            
        var orgResponse = await client.PostAsync("/organizations", orgContent);
        orgResponse.EnsureSuccessStatusCode();
        
        var organization = JsonSerializer.Deserialize<Organization>(
            await orgResponse.Content.ReadAsStringAsync(),
            _jsonSerializerOptions
        );
        
        // Create team request
        var createRequest = new CreateOrganizationTeamRequestData("Test Team");
        
        var content = new StringContent(
            JsonSerializer.Serialize(createRequest),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await client.PostAsync($"/organizations/{organization!.Id}/teams", content);
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        
        var team = JsonSerializer.Deserialize<OrganizationTeam>(responseString, _jsonSerializerOptions);
        Assert.NotNull(team);
        Assert.Equal("Test Team", team.Name);
        Assert.Equal(organization.Id, team.OrganizationId);
    }
    
    [Fact]
    public async Task Create_WithInvalidOrganizationId_ReturnsForbidden()
    {
        var client = CreateAuthenticatedClient();
        // Arrange
        var invalidOrgId = Guid.NewGuid();
        var createRequest = new CreateOrganizationTeamRequestData("Test Team");
        
        var content = new StringContent(
            JsonSerializer.Serialize(createRequest, _jsonSerializerOptions),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await client.PostAsync($"/organizations/{invalidOrgId}/teams", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    [Fact]
    public async Task Create_WithEmptyTeamName_ReturnsOk()
    {
        var client = CreateAuthenticatedClient();
        // Arrange - First create an organization
        var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Empty Team");
        
        var orgContent = new StringContent(
            JsonSerializer.Serialize(createOrgRequest),
            Encoding.UTF8,
            "application/json");
            
        var orgResponse = await client.PostAsync("/organizations", orgContent);
        orgResponse.EnsureSuccessStatusCode();
        
        var organization = JsonSerializer.Deserialize<Organization>(
            await orgResponse.Content.ReadAsStringAsync(),
            _jsonSerializerOptions
        );
        
        var createRequest = new CreateOrganizationTeamRequestData("");
        
        var content = new StringContent(
            JsonSerializer.Serialize(createRequest),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await client.PostAsync($"/organizations/{organization!.Id}/teams", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
} 