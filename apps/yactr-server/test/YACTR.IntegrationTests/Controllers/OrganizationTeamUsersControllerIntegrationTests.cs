using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using YACTR.DTO.RequestData.Organizations;
using YACTR.Data.Model.Organizations;
using YACTR.Data.Model.Authorization.Permissions;
using System.Net;

namespace YACTR.IntegrationTests.Controllers;

public class OrganizationTeamUsersControllerIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    public OrganizationTeamUsersControllerIntegrationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        
        // Setup authentication for protected endpoints
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyTestToken==");
    }
    
    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedTeamUser()
    {
        // Arrange - First create an organization
        var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Team Users");
        
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
        
        // Create a team
        var createTeamRequest = new CreateOrganizationTeamRequestData("Test Team for Users");
        
        var teamContent = new StringContent(
            JsonSerializer.Serialize(createTeamRequest),
            Encoding.UTF8,
            "application/json");
            
        var teamResponse = await _client.PostAsync($"/organizations/{organization!.Id}/teams", teamContent);
        teamResponse.EnsureSuccessStatusCode();
        
        var team = JsonSerializer.Deserialize<OrganizationTeam>(
            await teamResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        
        // Create team user request
        var createRequest = new CreateOrganizationTeamUserRequestData
        {
            UserId = Guid.NewGuid(),
            OrganizationTeamId = team!.Id,
            Permissions = new List<Permission> { Permission.TeamsRead, Permission.TeamsWrite }
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(createRequest),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await _client.PostAsync($"/organizations/{organization.Id}/teams/{team.Id}/users", content);
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        var teamUser = JsonSerializer.Deserialize<OrganizationTeamUser>(responseString, options);
        Assert.NotNull(teamUser);
        Assert.Equal(createRequest.UserId, teamUser.UserId);
        Assert.Equal(team.Id, teamUser.OrganizationTeamId);
        Assert.Equal(organization.Id, teamUser.OrganizationId);
        Assert.NotNull(teamUser.Permissions);
        Assert.Equal(2, teamUser.Permissions.Count);
    }
    
    [Fact]
    public async Task Create_WithInvalidOrganizationId_ReturnsNotFound()
    {
        // Arrange
        var invalidOrgId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var createRequest = new CreateOrganizationTeamUserRequestData
        {
            UserId = Guid.NewGuid(),
            OrganizationTeamId = teamId,
            Permissions = new List<Permission> { Permission.TeamsRead }
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(createRequest),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await _client.PostAsync($"/organizations/{invalidOrgId}/teams/{teamId}/users", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Create_WithInvalidTeamId_ReturnsNotFound()
    {
        // Arrange - First create an organization
        var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Invalid Team");
        
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
        
        var invalidTeamId = Guid.NewGuid();
        var createRequest = new CreateOrganizationTeamUserRequestData
        {
            UserId = Guid.NewGuid(),
            OrganizationTeamId = invalidTeamId,
            Permissions = new List<Permission> { Permission.TeamsRead }
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(createRequest),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await _client.PostAsync($"/organizations/{organization!.Id}/teams/{invalidTeamId}/users", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Create_WithEmptyPermissions_ReturnsBadRequest()
    {
        // Arrange - First create an organization and team
        var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Empty Permissions");
        
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
        
        var createTeamRequest = new CreateOrganizationTeamRequestData("Test Team for Empty Permissions");
        
        var teamContent = new StringContent(
            JsonSerializer.Serialize(createTeamRequest),
            Encoding.UTF8,
            "application/json");
            
        var teamResponse = await _client.PostAsync($"/organizations/{organization!.Id}/teams", teamContent);
        teamResponse.EnsureSuccessStatusCode();
        
        var team = JsonSerializer.Deserialize<OrganizationTeam>(
            await teamResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        
        var createRequest = new CreateOrganizationTeamUserRequestData
        {
            UserId = Guid.NewGuid(),
            OrganizationTeamId = team!.Id,
            Permissions = new List<Permission>()
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(createRequest),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await _client.PostAsync($"/organizations/{organization.Id}/teams/{team.Id}/users", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
} 