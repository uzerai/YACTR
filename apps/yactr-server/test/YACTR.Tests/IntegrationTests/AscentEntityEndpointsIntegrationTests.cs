using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using NodaTime;
using Shouldly;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Model.Authentication;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Endpoints.Ascents;

namespace YACTR.Tests.Endpoints;

[Collection("IntegrationTests")]
public class AscentEntityEndpointsIntegrationTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
{
    public User TestUserWithAscentPermissions = new()
    {
        Username = "test_user_with_ascent_permissions",
        Email = "test_user_ascents@test.dev",
        Auth0UserId = $"test|{Guid.NewGuid()}",
        PlatformPermissions = Enum.GetValues<Permission>()
    };

    // Let's provide the test user with permissions as a basis for all the tests
    protected override async ValueTask SetupAsync()
    {
        await base.SetupAsync();
        await fixture.GetEntityRepository<User>().CreateAsync(TestUserWithAscentPermissions, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task GetAllAscents_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateClient();

        // Act
        var (response, result) = await client.GETAsync<GetAllAscents, EmptyRequest, List<AscentResponse>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateAscent_WithValidAscentData_ReturnsCreatedAscent()
    {
        using var client = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        // Arrange
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));

        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Redpoint,
            CompletedAt: completedAt
        );

        // Act
        var (response, result) = await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(createRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.ShouldNotBeNull();
        result.Type.ShouldBe(AscentType.Redpoint);
        result.UserId.ShouldBe(TestUserWithAscentPermissions.Id);
        result.Route.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAscentById_WithValidAscentId_ReturnsAscent()
    {
        using var client = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        // Arrange - First create an ascent
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));

        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Onsight,
            CompletedAt: completedAt
        );

        var (createResponse, createdAscent) = await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act
        var getRequest = new GetAscentByIdRequest(createdAscent.Id);
        var (response, result) = await client.GETAsync<GetAscentById, GetAscentByIdRequest, AscentResponse>(getRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdAscent.Id);
        result.Type.ShouldBe(AscentType.Onsight);
        result.Route.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAscentById_WithNonExistentId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        // Act
        var getRequest = new GetAscentByIdRequest(Guid.NewGuid());
        var (response, _) = await client.GETAsync<GetAscentById, GetAscentByIdRequest, AscentResponse>(getRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateAscent_WithValidData_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        // Arrange - First create an ascent
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));

        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Project,
            CompletedAt: completedAt
        );

        var (createResponse, createdAscent) = await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act - Update the ascent
        var newCompletedAt = SystemClock.Instance.GetCurrentInstant();
        var updateRequest = new UpdateAscentRequest(
            AscentId: createdAscent.Id,
            Type: AscentType.Redpoint,
            CompletedAt: newCompletedAt
        );

        var (response, _) = await client.PUTAsync<UpdateAscent, UpdateAscentRequest, EmptyResponse>(updateRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify the update
        var getRequest = new GetAscentByIdRequest(createdAscent.Id);
        var (getResponse, updatedAscent) = await client.GETAsync<GetAscentById, GetAscentByIdRequest, AscentResponse>(getRequest);
        getResponse.IsSuccessStatusCode.ShouldBeTrue();
        updatedAscent.Type.ShouldBe(AscentType.Redpoint);
    }

    [Fact]
    public async Task UpdateAscent_WithNonExistentId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        // Act
        var updateRequest = new UpdateAscentRequest(
            AscentId: Guid.NewGuid(),
            Type: AscentType.Tick,
            CompletedAt: SystemClock.Instance.GetCurrentInstant()
        );

        var (response, _) = await client.PUTAsync<UpdateAscent, UpdateAscentRequest, EmptyResponse>(updateRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateAscent_WithDifferentUser_ReturnsForbidden()
    {
        using var clientWithPermissions = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);
        using var clientWithoutPermissions = fixture.CreateAuthenticatedClient();

        // Arrange - Create an ascent with the first user
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));

        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Tick,
            CompletedAt: completedAt
        );

        var (createResponse, createdAscent) = await clientWithPermissions.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act - Try to update with a different user
        var updateRequest = new UpdateAscentRequest(
            AscentId: createdAscent.Id,
            Type: AscentType.Redpoint,
            CompletedAt: SystemClock.Instance.GetCurrentInstant()
        );

        var (response, _) = await clientWithoutPermissions.PUTAsync<UpdateAscent, UpdateAscentRequest, EmptyResponse>(updateRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteAscent_WithValidId_ReturnsDeletedAscent()
    {
        using var client = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        // Arrange - First create an ascent
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));

        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Tick,
            CompletedAt: completedAt
        );

        var (createResponse, createdAscent) = await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act
        var deleteRequest = new DeleteAscentRequest(createdAscent.Id);
        var (response, result) = await client.DELETEAsync<DeleteAscent, DeleteAscentRequest, AscentResponse>(deleteRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdAscent.Id);

        // Verify the ascent is deleted
        var getRequest = new GetAscentByIdRequest(createdAscent.Id);
        var (getResponse, _) = await client.GETAsync<GetAscentById, GetAscentByIdRequest, AscentResponse>(getRequest);
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAscent_WithNonExistentId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        // Act
        var deleteRequest = new DeleteAscentRequest(Guid.NewGuid());
        var (response, _) = await client.DELETEAsync<DeleteAscent, DeleteAscentRequest, AscentResponse>(deleteRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAscent_WithDifferentUser_ReturnsForbidden()
    {
        using var clientWithPermissions = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);
        using var clientWithoutPermissions = fixture.CreateAuthenticatedClient();

        // Arrange - Create an ascent with the first user
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));

        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Tick,
            CompletedAt: completedAt
        );

        var (createResponse, createdAscent) = await clientWithPermissions.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act - Try to delete with a different user
        var deleteRequest = new DeleteAscentRequest(createdAscent.Id);
        var (response, _) = await clientWithoutPermissions.DELETEAsync<DeleteAscent, DeleteAscentRequest, AscentResponse>(deleteRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateAscent_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var createRequest = new CreateAscentRequest(
            RouteId: Guid.NewGuid(),
            Type: AscentType.Tick,
            CompletedAt: SystemClock.Instance.GetCurrentInstant()
        );

        // Act
        var (response, _) = await fixture.CreateClient().POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(createRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteAscent_WithoutAuthentication_ReturnsUnauthorized()
    {
        // This test covers the branch where the user is not authenticated
        // Arrange - Create an ascent with a valid user first
        using var validClient = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));

        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Tick,
            CompletedAt: completedAt
        );

        var (createResponse, createdAscent) = await validClient.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act - Try to delete without authentication (this tests the missing branch where ClaimValue returns null)
        var deleteRequest = new DeleteAscentRequest(createdAscent.Id);
        var (response, _) = await fixture.CreateClient().DELETEAsync<DeleteAscent, DeleteAscentRequest, AscentResponse>(deleteRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllAscents_WithMultipleAscents_ReturnsAllAscentsOrderedByCompletedAt()
    {
        using var client = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        // Arrange - Create test data and multiple ascents
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
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
        await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(ascentRequest1);

        // Create another ascent (middle)
        var ascentRequest2 = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Flash,
            CompletedAt: yesterday
        );
        await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(ascentRequest2);

        // Create another ascent (newest)
        var ascentRequest3 = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Onsight,
            CompletedAt: now
        );
        await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(ascentRequest3);

        // Act
        var (response, result) = await client.GETAsync<GetAllAscents, EmptyRequest, List<AscentResponse>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(3);

        // Find our test ascents and verify they're ordered by completion date (newest first)
        var ourAscents = result.Where(a => a.UserId == TestUserWithAscentPermissions.Id).ToList();
        ourAscents.Count.ShouldBe(3);

        // Should be ordered by CompletedAt descending (newest first)
        ourAscents[0].Type.ShouldBe(AscentType.Onsight);
        ourAscents[1].Type.ShouldBe(AscentType.Flash);
        ourAscents[2].Type.ShouldBe(AscentType.Redpoint);
    }
}