using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using NodaTime;
using Shouldly;
using YACTR.Data.Model.Achievement;
using YACTR.Data.Model.Authentication;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Location;
using YACTR.Endpoints;
using NetTopologySuite.Geometries;
using NetTopologySuite;

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

    private async Task<(Area area, Sector sector, Route route, Pitch pitch)> CreateTestClimbingData()
    {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        
        // Create Area
        var area = new Area()
        {
            Name = "Test Climbing Area",
            Description = "Test area for ascents",
            Location = geometryFactory.CreatePoint(new Coordinate(-122.4194, 37.7749)),
            Boundary = geometryFactory.CreateMultiPolygon(new[] {
                geometryFactory.CreatePolygon(new[] {
                    new Coordinate(-122.42, 37.77),
                    new Coordinate(-122.42, 37.78),
                    new Coordinate(-122.41, 37.78),
                    new Coordinate(-122.41, 37.77),
                    new Coordinate(-122.42, 37.77)
                })
            })
        };
        area = await fixture.GetEntityRepository<Area>().CreateAsync(area, TestContext.Current.CancellationToken);

        // Create Sector
        var sector = new Sector()
        {
            Name = "Test Sector",
            AreaId = area.Id,
            SectorArea = geometryFactory.CreatePolygon(new[] {
                new Coordinate(-122.42, 37.77),
                new Coordinate(-122.42, 37.775),
                new Coordinate(-122.415, 37.775),
                new Coordinate(-122.415, 37.77),
                new Coordinate(-122.42, 37.77)
            }),
            EntryPoint = geometryFactory.CreatePoint(new Coordinate(-122.4175, 37.7725))
        };
        sector = await fixture.GetEntityRepository<Sector>().CreateAsync(sector, TestContext.Current.CancellationToken);

        // Create Route
        var route = new Route()
        {
            Name = "Test Route",
            Description = "Test route for ascents",
            Grade = "5.10a",
            SectorId = sector.Id
        };
        route = await fixture.GetEntityRepository<Route>().CreateAsync(route, TestContext.Current.CancellationToken);

        // Create Pitch
        var pitch = new Pitch()
        {
            Name = "Test Pitch",
            Description = "Test pitch for ascents",
            Type = PitchType.Sport,
            SectorId = sector.Id
        };
        pitch = await fixture.GetEntityRepository<Pitch>().CreateAsync(pitch, TestContext.Current.CancellationToken);

        return (area, sector, route, pitch);
    }

    [Fact]
    public async Task GetAllAscents_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);
        
        // Act
        var (response, result) = await client.GETAsync<GetAllAscents, EmptyRequest, List<AscentResponse>>(new());
                
        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateAscent_WithValidRouteAscentData_ReturnsCreatedAscent()
    {
        using var client = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);
        
        // Arrange
        var (_, _, route, _) = await CreateTestClimbingData();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));
        
        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            PitchId: null,
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
        result.CompletedAt.ShouldBe(completedAt);
        result.UserId.ShouldBe(TestUserWithAscentPermissions.Id);
    }

    [Fact]
    public async Task CreateAscent_WithValidPitchAscentData_ReturnsCreatedAscent()
    {
        using var client = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);
        
        // Arrange
        var (_, _, _, pitch) = await CreateTestClimbingData();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));
        
        var createRequest = new CreateAscentRequest(
            RouteId: null,
            PitchId: pitch.Id,
            Type: AscentType.Flash,
            CompletedAt: completedAt
        );

        // Act
        var (response, result) = await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(createRequest);
        
        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.ShouldNotBeNull();
        result.Type.ShouldBe(AscentType.Flash);
        result.CompletedAt.ShouldBe(completedAt);
        result.UserId.ShouldBe(TestUserWithAscentPermissions.Id);
    }

    [Fact]
    public async Task CreateAscent_WithNeitherRouteIdNorPitchId_ReturnsBadRequest()
    {
        using var client = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);
        
        // Arrange
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));
        
        var createRequest = new CreateAscentRequest(
            RouteId: null,
            PitchId: null,
            Type: AscentType.Tick,
            CompletedAt: completedAt
        );

        // Act
        var (response, _) = await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(createRequest);
        
        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAscentById_WithValidRouteAscentId_ReturnsAscent()
    {
        using var client = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);
        
        // Arrange - First create a route ascent
        var (_, _, route, _) = await CreateTestClimbingData();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));
        
        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            PitchId: null,
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
    }

    [Fact]
    public async Task GetAscentById_WithValidPitchAscentId_ReturnsAscent()
    {
        using var client = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);
        
        // Arrange - First create a pitch ascent
        var (_, _, _, pitch) = await CreateTestClimbingData();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));
        
        var createRequest = new CreateAscentRequest(
            RouteId: null,
            PitchId: pitch.Id,
            Type: AscentType.Seconded,
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
        result.Type.ShouldBe(AscentType.Seconded);
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
        var (_, _, route, _) = await CreateTestClimbingData();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));
        
        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            PitchId: null,
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
        updatedAscent.CompletedAt.ShouldBe(newCompletedAt);
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
        var (_, _, route, _) = await CreateTestClimbingData();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));
        
        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            PitchId: null,
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
    public async Task DeleteAscent_WithValidId_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);
        
        // Arrange - First create an ascent
        var (_, _, route, _) = await CreateTestClimbingData();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));
        
        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            PitchId: null,
            Type: AscentType.Tick,
            CompletedAt: completedAt
        );
        
        var (createResponse, createdAscent) = await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();
        
        // Act
        var deleteRequest = new DeleteAscentRequest(createdAscent.Id);
        var (response, _) = await client.DELETEAsync<DeleteAscent, DeleteAscentRequest, AscentResponse>(deleteRequest);
        
        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        
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
        var (response, _) = await client.DELETEAsync<DeleteAscent, DeleteAscentRequest, EmptyResponse>(deleteRequest);
        
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
        var (_, _, route, _) = await CreateTestClimbingData();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));
        
        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            PitchId: null,
            Type: AscentType.Tick,
            CompletedAt: completedAt
        );
        
        var (createResponse, createdAscent) = await clientWithPermissions.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();
        
        // Act - Try to delete with a different user
        var deleteRequest = new DeleteAscentRequest(createdAscent.Id);
        var (response, _) = await clientWithoutPermissions.DELETEAsync<DeleteAscent, DeleteAscentRequest, EmptyResponse>(deleteRequest);
        
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
            PitchId: null,
            Type: AscentType.Tick,
            CompletedAt: SystemClock.Instance.GetCurrentInstant()
        );
        
        // Act
        var (response, _) = await fixture.AnonymousClient.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(createRequest);
        
        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllAscents_WithMultipleAscentTypes_ReturnsAllAscentsOrderedByCompletedAt()
    {
        using var client = fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);
        
        // Arrange - Create test data and multiple ascents
        var (_, _, route, pitch) = await CreateTestClimbingData();
        
        var now = SystemClock.Instance.GetCurrentInstant();
        var yesterday = now.Minus(Duration.FromDays(1));
        var twoDaysAgo = now.Minus(Duration.FromDays(2));
        
        // Create route ascent (oldest)
        var routeAscentRequest = new CreateAscentRequest(
            RouteId: route.Id,
            PitchId: null,
            Type: AscentType.Redpoint,
            CompletedAt: twoDaysAgo
        );
        await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(routeAscentRequest);
        
        // Create pitch ascent (middle)
        var pitchAscentRequest = new CreateAscentRequest(
            RouteId: null,
            PitchId: pitch.Id,
            Type: AscentType.Flash,
            CompletedAt: yesterday
        );
        await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(pitchAscentRequest);
        
        // Create another route ascent (newest)
        var anotherRouteAscentRequest = new CreateAscentRequest(
            RouteId: route.Id,
            PitchId: null,
            Type: AscentType.Onsight,
            CompletedAt: now
        );
        await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(anotherRouteAscentRequest);
        
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
        ourAscents[0].CompletedAt.ShouldBe(now);
        ourAscents[0].Type.ShouldBe(AscentType.Onsight);
        
        ourAscents[1].CompletedAt.ShouldBe(yesterday);
        ourAscents[1].Type.ShouldBe(AscentType.Flash);
        
        ourAscents[2].CompletedAt.ShouldBe(twoDaysAgo);
        ourAscents[2].Type.ShouldBe(AscentType.Redpoint);
    }
} 