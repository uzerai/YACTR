using FastEndpoints.Testing;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Organizations;

namespace YACTR.Api.Tests.EndpointTests.Organizations.Teams.Users;

public abstract class OrganizationTeamUserEndpointTestsBase(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    protected ApiTestClassFixture Fixture => fixture;

    protected Organization Organization = null!;
    protected OrganizationTeam OrgTeam = null!;
    protected OrganizationUser OrganizationUser = null!;

    protected User AllPermissionsUser = new()
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
        AllPermissionsUser = user;

        Organization = await fixture.GetEntityRepository<Organization>()
            .CreateAsync(new()
            {
                Name = "Test Organization",
            }, TestContext.Current.CancellationToken);

        OrgTeam = await fixture.GetEntityRepository<OrganizationTeam>()
            .CreateAsync(new()
            {
                OrganizationId = Organization.Id,
                Name = "Org team 1"
            }, TestContext.Current.CancellationToken);

        OrganizationUser = await fixture.GetRepository<OrganizationUser>()
            .CreateAsync(new()
            {
                UserId = user.Id,
                OrganizationId = Organization.Id,
                Permissions = Enum.GetValues<Permission>()
            }, TestContext.Current.CancellationToken);
    }
}
