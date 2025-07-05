using YACTR.DTO.RequestData.Organizations;
using YACTR.Data.Model.Organizations;
using YACTR.Data.Model.Authorization.Permissions;
using System.Net;
using YACTR.Data.Model.Authentication;
using Microsoft.EntityFrameworkCore;

namespace YACTR.IntegrationTests.Controllers;

[Collection("IntegrationTests")]
public class OrganizationTeamUsersControllerIntegrationTests : IntegrationTestClassFixture
{
    public OrganizationTeamUsersControllerIntegrationTests(TestWebApplicationFactory factory) : base(factory)
    {        
    }
    
    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedTeamUser()
    {
        var client = CreateAuthenticatedClient();
        // Arrange - First create an organization
        var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Team Users");
        
        var orgContent = SerializeJsonFromRequestData(createOrgRequest);
            
        var orgResponse = await client.PostAsync("/organizations", orgContent);
        orgResponse.EnsureSuccessStatusCode();
        
        var organization = await DeserializeEntityFromResponse<Organization>(orgResponse);
        
        // Create a team
        var createTeamRequest = new CreateOrganizationTeamRequestData("Test Team for Users");
        
        var teamContent = SerializeJsonFromRequestData(createTeamRequest);
            
        var teamResponse = await client.PostAsync($"/organizations/{organization!.Id}/teams", teamContent);
        teamResponse.EnsureSuccessStatusCode();
        
        var team = await DeserializeEntityFromResponse<OrganizationTeam>(teamResponse);

        var user = await _databaseContext.Set<User>().AsNoTracking().Where(user => user.Username == TestAuthenticationHandler.DEFAULT_TEST_USER.Username).FirstOrDefaultAsync();
        
        // Create team user request
        var createRequest = new CreateOrganizationTeamUserRequestData
        {
            UserId = user!.Id,
            Permissions = new List<Permission> { Permission.TeamsRead, Permission.TeamsWrite }
        };
        
        var content = SerializeJsonFromRequestData(createRequest);
            
        // Act
        var response = await client.PostAsync($"/organizations/{organization.Id}/teams/{team!.Id}/users", content);
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var teamUser = await DeserializeEntityFromResponse<OrganizationTeamUser>(response);
        Assert.NotNull(teamUser);
        Assert.Equal(createRequest.UserId, teamUser.UserId);
        Assert.Equal(team.Id, teamUser.OrganizationTeamId);
        Assert.Equal(organization.Id, teamUser.OrganizationId);
        Assert.NotNull(teamUser.Permissions);
        Assert.Equal(2, teamUser.Permissions.Count);
    }

    [Fact]
    // This is Forbidden because the user CANNOT have permissions 
    // to add team users on a non-existent organization.
    // (because the organization does not exist)
    public async Task Create_WithInvalidOrganizationId_ReturnsForbidden()
    {
        var client = CreateAuthenticatedClient();
        // Arrange
        var invalidOrgId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var createRequest = new CreateOrganizationTeamUserRequestData
        {
            UserId = Guid.NewGuid(),
            Permissions = new List<Permission> { Permission.TeamsRead }
        };

        var content = SerializeJsonFromRequestData(createRequest);

        // Act
        var response = await client.PostAsync($"/organizations/{invalidOrgId}/teams/{teamId}/users", content);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    [Fact]
    public async Task Create_WithInvalidTeamId_ReturnsFailedDepedency()
    {
        var client = CreateAuthenticatedClient();
        // Arrange - First create an organization
        var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Invalid Team");
        
        var orgContent = SerializeJsonFromRequestData(createOrgRequest);
            
        var orgResponse = await client.PostAsync("/organizations", orgContent);
        orgResponse.EnsureSuccessStatusCode();
        
        var organization = await DeserializeEntityFromResponse<Organization>(orgResponse);
        
        var invalidTeamId = Guid.NewGuid();
        var createRequest = new CreateOrganizationTeamUserRequestData
        {
            UserId = Guid.NewGuid(),
            Permissions = new List<Permission> { Permission.TeamsRead }
        };
        
        var content = SerializeJsonFromRequestData(createRequest);
            
        // Act
        var response = await client.PostAsync($"/organizations/{organization!.Id}/teams/{invalidTeamId}/users", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.FailedDependency, response.StatusCode);
    }
    
    [Fact]
    public async Task Create_WithEmptyPermissions_ReturnsOK()
    {
        var client = CreateAuthenticatedClient();
        // Arrange - First create an organization and team
        var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Empty Permissions");
        
        var orgContent = SerializeJsonFromRequestData(createOrgRequest);
            
        var orgResponse = await client.PostAsync("/organizations", orgContent);
        orgResponse.EnsureSuccessStatusCode();
        
        var organization = await DeserializeEntityFromResponse<Organization>(orgResponse);
        
        var createTeamRequest = new CreateOrganizationTeamRequestData("Test Team for Empty Permissions");
        
        var teamContent = SerializeJsonFromRequestData(createTeamRequest);
            
        var teamResponse = await client.PostAsync($"/organizations/{organization!.Id}/teams", teamContent);
        teamResponse.EnsureSuccessStatusCode();
        
        var team = await DeserializeEntityFromResponse<OrganizationTeam>(teamResponse);
        
        var user = await _databaseContext.Set<User>().AsNoTracking().Where(user => user.Username == TestAuthenticationHandler.DEFAULT_TEST_USER.Username).FirstOrDefaultAsync();
        var createRequest = new CreateOrganizationTeamUserRequestData
        {
            UserId = user!.Id,
            Permissions = new List<Permission>()
        };
        
        var content = SerializeJsonFromRequestData(createRequest);
            
        // Act
        var response = await client.PostAsync($"/organizations/{organization.Id}/teams/{team!.Id}/users", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
} 