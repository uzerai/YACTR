using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Data.Model.Organizations;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Authentication;
using YACTR.Endpoints.Organizations;

namespace YACTR.Tests.EndpointTests;

[Collection("IntegrationTests")]
public class OrganizationTeamUserEntityEndpointsIntegrationTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
{
    public Organization _organization = null!;
    public OrganizationTeam _orgTeam = null!;
    public OrganizationUser _organizationUser = null!;

    public User AllPermissionsUser = new()
    {
        Username = "test_user_with_all_permissions",
        Email = "test_user@test.dev",
        Auth0UserId = $"test|{Guid.NewGuid()}",
        PlatformPermissions = Enum.GetValues<Permission>()
    };

    protected override async ValueTask SetupAsync()
    {
        await base.SetupAsync();

        User user = await fixture.GetEntityRepository<User>()
            .CreateAsync(AllPermissionsUser, TestContext.Current.CancellationToken);

        _organization = await fixture.GetEntityRepository<Organization>()
            .CreateAsync(new()
            {
                Name = "Test Organization",
            }, TestContext.Current.CancellationToken);

        _orgTeam = await fixture.GetEntityRepository<OrganizationTeam>()
            .CreateAsync(new()
            {
                OrganizationId = _organization.Id,
                Name = "Org team 1"
            }, TestContext.Current.CancellationToken);


        // Organization user with all permissions possible.
        _organizationUser = await fixture.GetRepository<OrganizationUser>()
            .CreateAsync(new()
            {
                UserId = user.Id,
                OrganizationId = _organization.Id,
                Permissions = Enum.GetValues<Permission>()
            }, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task CreateTeamUser_WithValidData_ReturnsCreatedTeamUser()
    {
        using var client = fixture.CreateAuthenticatedClient(AllPermissionsUser);

        var noOrgUser = await fixture.GetEntityRepository<User>()
            .CreateAsync(new User()
            {
                Auth0UserId = $"auth0|{Guid.NewGuid()}",
                Email = "orgless@noorg.com",
                Username = "no org user"
            }, TestContext.Current.CancellationToken);

        var request = new CreateOrganizationTeamUserRequest(_organization.Id, _orgTeam.Id, noOrgUser.Id, []);
        var (response, created) = await client.POSTAsync<CreateOrganizationTeamUser, CreateOrganizationTeamUserRequest, OrganizationTeamUser>(request);
        response.IsSuccessStatusCode.ShouldBeTrue();
    }

    [Fact]
    public async Task Create_WithInvalidTeamId_ReturnsFailedDependency()
    {
        using var client = fixture.CreateAuthenticatedClient(AllPermissionsUser);

        var noOrgUser = await fixture.GetEntityRepository<User>()
            .CreateAsync(new User()
            {
                Auth0UserId = $"auth0|{Guid.NewGuid()}",
                Email = "orgless@noorg.com",
                Username = "no org user"
            }, TestContext.Current.CancellationToken);

        var request = new CreateOrganizationTeamUserRequest(_organization.Id, Guid.CreateVersion7(), noOrgUser.Id, []);
        var (response, created) = await client.POSTAsync<CreateOrganizationTeamUser, CreateOrganizationTeamUserRequest, OrganizationTeamUser>(request);
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.FailedDependency);
    }

    [Fact]
    public async Task Create_WithNoPermissions_ReturnsForbidden()
    {
        var noPermissionUser = await fixture.GetEntityRepository<User>()
            .CreateAsync(new()
            {
                Username = "test_user_with_no_permissions",
                Email = "test_user_no_perms@test.dev",
                Auth0UserId = $"test|no_org_no_permissions",
                PlatformPermissions = []
            }, TestContext.Current.CancellationToken);

        using var client = fixture.CreateAuthenticatedClient(noPermissionUser);
        var noOrgUser = await fixture.GetEntityRepository<User>()
            .CreateAsync(new User()
            {
                Auth0UserId = "auth0|newUserNoOrgs",
                Email = "orgless@noorg.com",
                Username = "no org user"
            }, TestContext.Current.CancellationToken);
        
        var request = new CreateOrganizationTeamUserRequest(_organization.Id, Guid.CreateVersion7(), noOrgUser.Id, []);
        var (response, created) = await client.POSTAsync<CreateOrganizationTeamUser, CreateOrganizationTeamUserRequest, OrganizationTeamUser>(request);
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        
    }
}