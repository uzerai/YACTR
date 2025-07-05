using YACTR.Data.Model.Organizations;
using System.Net;
using YACTR.Endpoints.Organizations;
using YACTR.Endpoints.Organizations.Teams;

namespace YACTR.Tests.Controllers;

[Collection("IntegrationTests")]
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

        var orgContent = SerializeJsonFromRequestData(createOrgRequest);

        var orgResponse = await client.PostAsync("/organizations", orgContent);
        orgResponse.EnsureSuccessStatusCode();
        
        var organization = await DeserializeEntityFromResponse<Organization>(orgResponse);
        
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
        
        var orgContent = SerializeJsonFromRequestData(createOrgRequest);
            
        var orgResponse = await client.PostAsync("/organizations", orgContent);
        orgResponse.EnsureSuccessStatusCode();

        var organization = await DeserializeEntityFromResponse<Organization>(orgResponse);
        
        // Create team request
        var createRequest = new CreateOrganizationTeamRequest(organization.Id, "Test Team");
        
        var content = SerializeJsonFromRequestData(createRequest);
            
        // Act
        var response = await client.PostAsync($"/organizations/{organization!.Id}/teams", content);
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var team = await DeserializeEntityFromResponse<OrganizationTeam>(response);
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
        var createRequest = new CreateOrganizationTeamRequest(invalidOrgId, "Test Team");
        
        var content = SerializeJsonFromRequestData(createRequest);
            
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
        
        var orgContent = SerializeJsonFromRequestData(createOrgRequest);
            
        var orgResponse = await client.PostAsync("/organizations", orgContent);
        orgResponse.EnsureSuccessStatusCode();
        
        var organization = await DeserializeEntityFromResponse<Organization>(orgResponse);
        
        var createRequest = new CreateOrganizationTeamRequest(organization.Id, "");
        
        var content = SerializeJsonFromRequestData(createRequest);
            
        // Act
        var response = await client.PostAsync($"/organizations/{organization!.Id}/teams", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
} 