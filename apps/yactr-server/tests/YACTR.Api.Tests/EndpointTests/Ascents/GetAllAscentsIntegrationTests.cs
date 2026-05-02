using NodaTime;
using Shouldly;
using YACTR.Api.Endpoints.Ascents;
using YACTR.Api.Pagination;
using YACTR.Domain.Model.Achievement;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Api.Tests.EndpointTests.Ascents;

[Collection("IntegrationTests")]
public class GetAllAscentsIntegrationTests(ApiTestClassFixture fixture) : AscentEndpointTestsBase(fixture)
{
    [Fact]
    public async Task GetAllAscents_ReturnsSuccessStatusCode()
    {
        using var client = Fixture.CreateClient();

        // Act
        var (response, result) = await client.GETAsync<GetAllAscents, GetAllAscentsRequest, PaginatedResponse<GetAllAscentsResponseItem>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAllAscents_WithMultipleAscents_ReturnsAllAscentsOrderedByCompletedAt()
    {
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        // Arrange - Create test data and multiple ascents
        var (_, _, routes) = await Fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();

        var now = SystemClock.Instance.GetCurrentInstant();
        var yesterday = now.Minus(Duration.FromDays(1));
        var twoDaysAgo = now.Minus(Duration.FromDays(2));

        // Create ascent (oldest)
        var ascentRequest1 = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Redpoint,
            CompletedAt: twoDaysAgo
        );
        await client.POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(ascentRequest1);

        // Create another ascent (middle)
        var ascentRequest2 = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Flash,
            CompletedAt: yesterday
        );
        await client.POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(ascentRequest2);

        // Create another ascent (newest)
        var ascentRequest3 = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Onsight,
            CompletedAt: now
        );
        await client.POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(ascentRequest3);

        // Act
        var (response, result) = await client.GETAsync<GetAllAscents, GetAllAscentsRequest, PaginatedResponse<GetAllAscentsResponseItem>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.Count.ShouldBeGreaterThanOrEqualTo(3);

        // Find our test ascents and verify they're ordered by completion date (newest first)
        var ourAscents = result.Items.Where(a => a.UserId == TestUserWithAscentPermissions.Id).ToList();
        ourAscents.Count.ShouldBe(3);

        // Should be ordered by CompletedAt descending (newest first)
        ourAscents[0].Type.ShouldBe(AscentType.Onsight);
        ourAscents[1].Type.ShouldBe(AscentType.Flash);
        ourAscents[2].Type.ShouldBe(AscentType.Redpoint);
    }

    [Fact]
    public async Task GetAllAscents_WithRouteIdFilter_ReturnsMatchingAscents()
    {
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        var (_, _, routes) = await Fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var primaryRoute = routes.First();
        var secondaryRoute = routes.Skip(1).First();
        var now = Fixture.TestClock.GetCurrentInstant();

        await client.POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(new(primaryRoute.Id, AscentType.Onsight, now));
        await client.POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(new(secondaryRoute.Id, AscentType.Redpoint, now.Minus(Duration.FromHours(1))));

        var (response, result) = await client.GETAsync<GetAllAscents, GetAllAscentsRequest, PaginatedResponse<GetAllAscentsResponseItem>>(new()
        {
            RouteId = primaryRoute.Id
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.All(a => a.Route is not null && a.Route.Id == primaryRoute.Id).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAllAscents_WithUserIdFilter_ReturnsMatchingAscents()
    {
        var secondaryUser = new User
        {
            Username = "test_user_with_ascent_permissions_2",
            Email = "test_user_ascents_2@test.dev",
            Auth0UserId = $"test|{Guid.NewGuid()}",
            PlatformPermissions = Enum.GetValues<Permission>()
        };

        using var firstUserClient = Fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);
        using var secondUserClient = Fixture.CreateAuthenticatedClient(secondaryUser);

        var (_, _, routes) = await Fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();
        var now = Fixture.TestClock.GetCurrentInstant();

        await firstUserClient.POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(new(route.Id, AscentType.Onsight, now));
        await secondUserClient.POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(new(route.Id, AscentType.Flash, now.Minus(Duration.FromHours(2))));

        var (response, result) = await firstUserClient.GETAsync<GetAllAscents, GetAllAscentsRequest, PaginatedResponse<GetAllAscentsResponseItem>>(new()
        {
            UserId = TestUserWithAscentPermissions.Id
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.All(a => a.UserId == TestUserWithAscentPermissions.Id).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAllAscents_WithTypeFilter_ReturnsMatchingAscents()
    {
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        var (_, _, routes) = await Fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();
        var now = Fixture.TestClock.GetCurrentInstant();

        await client.POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(new(route.Id, AscentType.Onsight, now));
        await client.POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(new(route.Id, AscentType.Redpoint, now.Minus(Duration.FromHours(1))));

        var (response, result) = await client.GETAsync<GetAllAscents, GetAllAscentsRequest, PaginatedResponse<GetAllAscentsResponseItem>>(new()
        {
            Type = AscentType.Onsight
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.All(a => a.Type == AscentType.Onsight).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAllAscents_WithCreatedBeforeFilter_ReturnsOlderAscents()
    {
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        var (_, _, routes) = await Fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();
        var completedAt = Instant.FromUtc(2025, 2, 10, 0, 0);
        var earlyCreatedAt = Instant.FromUtc(2025, 2, 1, 0, 0);
        var lateCreatedAt = Instant.FromUtc(2025, 2, 10, 0, 0);

        Fixture.SetTestClock(earlyCreatedAt);
        var (_, olderAscent) = await client.POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(new(route.Id, AscentType.Onsight, completedAt));
        Fixture.SetTestClock(lateCreatedAt);
        await client.POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(new(route.Id, AscentType.Redpoint, completedAt.Plus(Duration.FromDays(1))));

        var (response, result) = await client.GETAsync<GetAllAscents, GetAllAscentsRequest, PaginatedResponse<GetAllAscentsResponseItem>>(new()
        {
            CreatedBefore = Instant.FromUtc(2025, 2, 5, 0, 0)
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(a => a.Id == olderAscent.Id);
        result.Items.All(a => a.Id == olderAscent.Id).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAllAscents_WithCreatedAfterFilter_ReturnsNewerAscents()
    {
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        var (_, _, routes) = await Fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();
        var completedAt = Instant.FromUtc(2025, 3, 10, 0, 0);
        var earlyCreatedAt = Instant.FromUtc(2025, 3, 1, 0, 0);
        var lateCreatedAt = Instant.FromUtc(2025, 3, 10, 0, 0);

        Fixture.SetTestClock(earlyCreatedAt);
        await client.POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(new(route.Id, AscentType.Onsight, completedAt));
        Fixture.SetTestClock(lateCreatedAt);
        var (_, newerAscent) = await client.POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(new(route.Id, AscentType.Flash, completedAt.Plus(Duration.FromDays(1))));

        var (response, result) = await client.GETAsync<GetAllAscents, GetAllAscentsRequest, PaginatedResponse<GetAllAscentsResponseItem>>(new()
        {
            CreatedAfter = Instant.FromUtc(2025, 3, 5, 0, 0)
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(a => a.Id == newerAscent.Id);
        result.Items.All(a => a.Id == newerAscent.Id).ShouldBeTrue();
    }
}
