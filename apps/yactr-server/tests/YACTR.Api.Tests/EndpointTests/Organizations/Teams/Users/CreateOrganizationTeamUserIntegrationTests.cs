using System.Net;
using Shouldly;
using YACTR.Api.Endpoints.Organizations;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Organizations;

namespace YACTR.Api.Tests.EndpointTests.Organizations.Teams.Users;

[Collection("IntegrationTests")]
public class CreateOrganizationTeamUserIntegrationTests(ApiTestClassFixture fixture) : OrganizationTeamUserEndpointTestsBase(fixture)
{
    [Fact]
    public async Task CreateTeamUser_WithValidData_ReturnsCreatedTeamUser()
    {
        using var client = Fixture.CreateAuthenticatedClient(AllPermissionsUser);

        var noOrgUser = await Fixture.GetEntityRepository<User>()
            .CreateAsync(new User()
            {
                Auth0UserId = $"auth0|{Guid.NewGuid()}",
                Email = "orgless@noorg.com",
                Username = "no org user"
            }, TestContext.Current.CancellationToken);

        var request = new CreateOrganizationTeamUserRequest(Organization.Id, OrgTeam.Id, noOrgUser.Id, []);
        var (response, created) = await client.POSTAsync<CreateOrganizationTeamUser, CreateOrganizationTeamUserRequest, OrganizationTeamUser>(request);
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_WithInvalidTeamId_ReturnsFailedDependency()
    {
        using var client = Fixture.CreateAuthenticatedClient(AllPermissionsUser);

        var noOrgUser = await Fixture.GetEntityRepository<User>()
            .CreateAsync(new User()
            {
                Auth0UserId = $"auth0|{Guid.NewGuid()}",
                Email = "orgless@noorg.com",
                Username = "no org user"
            }, TestContext.Current.CancellationToken);

        var request = new CreateOrganizationTeamUserRequest(Organization.Id, Guid.CreateVersion7(), noOrgUser.Id, []);
        var (response, created) = await client.POSTAsync<CreateOrganizationTeamUser, CreateOrganizationTeamUserRequest, OrganizationTeamUser>(request);
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_WithNoPermissions_ReturnsForbidden()
    {
        var noPermissionUser = await Fixture.GetEntityRepository<User>()
            .CreateAsync(new()
            {
                Username = "test_user_with_no_permissions",
                Email = "test_user_no_perms@test.dev",
                Auth0UserId = $"test|no_org_no_permissions",
                PlatformPermissions = []
            }, TestContext.Current.CancellationToken);

        using var client = Fixture.CreateAuthenticatedClient(noPermissionUser);
        var noOrgUser = await Fixture.GetEntityRepository<User>()
            .CreateAsync(new User()
            {
                Auth0UserId = "auth0|newUserNoOrgs",
                Email = "orgless@noorg.com",
                Username = "no org user"
            }, TestContext.Current.CancellationToken);

        var request = new CreateOrganizationTeamUserRequest(Organization.Id, Guid.CreateVersion7(), noOrgUser.Id, []);
        var (response, created) = await client.POSTAsync<CreateOrganizationTeamUser, CreateOrganizationTeamUserRequest, OrganizationTeamUser>(request);
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
