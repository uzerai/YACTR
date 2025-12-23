using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Organizations;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Authentication;
using YACTR.Endpoints.Organizations;
using YACTR.Endpoints.Organizations.Teams;

namespace YACTR.Tests.EndpointTests;

[Collection("IntegrationTests")]
public class OrganizationTeamUserEntityEndpointsIntegrationTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
{
    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedTeamUser()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange - First create an organization
        var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Team Users");
        var (orgResponse, organization) = await client.POSTAsync<CreateOrganization, CreateOrganizationRequestData, Organization>(createOrgRequest);
        orgResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Create a team
        var createTeamRequest = new CreateOrganizationTeamRequest(organization.Id, "Test Team for Users");
        var (teamResponse, team) = await client.POSTAsync<CreateOrganizationTeam, CreateOrganizationTeamRequest, OrganizationTeam>(createTeamRequest);
        teamResponse.IsSuccessStatusCode.ShouldBeTrue();

        var user = await fixture.GetEntityRepository<User>()
          .BuildReadonlyQuery()
          .Where(user => user.Username == TestAuthenticationHandler.DEFAULT_TEST_USER.Username)
          .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Create team user request
        var createRequest = new CreateOrganizationTeamUserRequest(organization.Id, team.Id, user!.Id, [Permission.TeamsRead, Permission.TeamsWrite]);

        // Act
        var (response, result) = await client.POSTAsync<CreateOrganizationTeamUser, CreateOrganizationTeamUserRequest, OrganizationTeamUser>(createRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.UserId.ShouldBe(createRequest.UserId);
        result.OrganizationTeamId.ShouldBe(team.Id);
        result.OrganizationId.ShouldBe(organization.Id);
        result.Permissions.ShouldNotBeNull();
        result.Permissions.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Create_WithInvalidTeamId_ReturnsFailedDependency()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange - First create an organization
        var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Invalid Team");
        var (orgResponse, organization) = await client.POSTAsync<CreateOrganization, CreateOrganizationRequestData, Organization>(createOrgRequest);
        orgResponse.IsSuccessStatusCode.ShouldBeTrue();

        var invalidTeamId = Guid.NewGuid();
        var user = await fixture.GetEntityRepository<User>()
          .BuildReadonlyQuery()
          .Where(user => user.Username == TestAuthenticationHandler.DEFAULT_TEST_USER.Username)
          .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);
        
        var createRequest = new CreateOrganizationTeamUserRequest(organization.Id, invalidTeamId, user!.Id, [Permission.TeamsRead]);

        // Act
        var (response, _) = await client.POSTAsync<CreateOrganizationTeamUser, CreateOrganizationTeamUserRequest, OrganizationTeamUser>(createRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.FailedDependency);
    }

    [Fact]
    public async Task Create_WithEmptyPermissions_ReturnsOK()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange - First create an organization and team
        var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Empty Permissions");
        var (orgResponse, organization) = await client.POSTAsync<CreateOrganization, CreateOrganizationRequestData, Organization>(createOrgRequest);
        orgResponse.IsSuccessStatusCode.ShouldBeTrue();

        var createTeamRequest = new CreateOrganizationTeamRequest(organization.Id, "Test Team for Empty Permissions");
        var (teamResponse, team) = await client.POSTAsync<CreateOrganizationTeam, CreateOrganizationTeamRequest, OrganizationTeam>(createTeamRequest);
        teamResponse.IsSuccessStatusCode.ShouldBeTrue();

        var user = await fixture.GetEntityRepository<User>()
          .BuildReadonlyQuery()
          .Where(user => user.Username == TestAuthenticationHandler.DEFAULT_TEST_USER.Username)
          .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

        var createRequest = new CreateOrganizationTeamUserRequest(organization.Id, team.Id, user!.Id, []);

        // Act
        var (response, result) = await client.POSTAsync<CreateOrganizationTeamUser, CreateOrganizationTeamUserRequest, OrganizationTeamUser>(createRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
    }
}