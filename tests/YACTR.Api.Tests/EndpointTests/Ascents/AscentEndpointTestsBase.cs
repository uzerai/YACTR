using FastEndpoints.Testing;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Api.Tests.EndpointTests.Ascents;

public abstract class AscentEndpointTestsBase(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    protected ApiTestClassFixture Fixture => fixture;

    protected User TestUserWithAscentPermissions = new()
    {
        Username = "test_user_with_ascent_permissions",
        Email = "test_user_ascents@test.dev",
        Auth0UserId = $"test|{Guid.NewGuid()}",
        PlatformPermissions = Enum.GetValues<Permission>()
    };

    protected override async ValueTask SetupAsync()
    {
        await base.SetupAsync();
        await fixture.GetEntityRepository<User>().CreateAsync(TestUserWithAscentPermissions, TestContext.Current.CancellationToken);
    }
}
