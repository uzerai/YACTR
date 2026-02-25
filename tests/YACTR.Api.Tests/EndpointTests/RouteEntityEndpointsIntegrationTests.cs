using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Routes;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Tests.EndpointTests;

[Collection("IntegrationTests")]
public class RouteEntityEndpointsIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{

    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();

        var (response, result) = await client.GETAsync<GetAllRoutes, EmptyRequest, List<Route>>(new());

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedAndBody()
    {
        using var client = fixture.CreateAuthenticatedClient();

        var (_, sector, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var jpegImage = await fixture.TestDataSeeder.CreateImageAsync();
        var svgOverlayImage = await fixture.TestDataSeeder.CreateImageAsync(TestDataConstants.MINIMAL_SVG);

        var routeReq = new RouteRequestData(
            SectorId: sector.Id,
            Type: ClimbingType.Sport,
            Pitches: [
                new RoutePitchRequestData(
                    Id: null,
                    Name: "Test Pitch",
                    Type: ClimbingType.Sport,
                    Description: "A sample pitch",
                    Grade: 500,
                    GearCount: 1,
                    Height: 10,
                    PitchOrder: 1
                )
            ],
            Name: "Test Route Create",
            Description: "A sample route",
            Grade: 500,
            FirstAscentClimberName: null,
            BolterName: null,
            TopoImageId: jpegImage.Id,
            TopoImageOverlayId: svgOverlayImage.Id
        );

        var (response, created) = await client.POSTAsync<CreateRoute, RouteRequestData, RouteResponse>(routeReq);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        created.ShouldNotBeNull();
        created.Id.ShouldNotBe(Guid.Empty);
        created.Name.ShouldBe("Test Route Create");
        created.TopoImageUrl.ShouldNotBeNull();
        created.TopoImageOverlayUrl.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsRoute()
    {
        using var client = fixture.CreateAuthenticatedClient();

        var (_, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var created = routes.First();

        // Create a route first
        // Fetch it
        var getReq = new GetRouteByIdRequest(created.Id);
        var (response, result) = await client.GETAsync<GetRouteById, GetRouteByIdRequest, RouteResponse>(getReq);

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Name.ShouldBe(created.Name);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var getReq = new GetRouteByIdRequest(Guid.NewGuid());
        var (response, _) = await client.GETAsync<GetRouteById, GetRouteByIdRequest, RouteResponse>(getReq);
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_WithValidId_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();

        var (_, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var created = routes.First();

        // Create route to update
        RouteRequestData routeReq = new(
            SectorId: sector.Id,
            Type: ClimbingType.Sport,
            Pitches: [],
            Name: "Test Route Update",
            Description: "Updated Test Route Description",
            Grade: 0,
            FirstAscentClimberName: "Updated Test Route First Ascent Climber Name",
            BolterName: "Updated Test Route Bolter Name"
        );

        var updateReq = new UpdateRouteRequest { RouteId = created.Id, Route = routeReq };
        var (response, _) = await client.PUTAsync<UpdateRoute, UpdateRouteRequest, EmptyResponse>(updateReq);
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var updateReq = new UpdateRouteRequest { RouteId = Guid.NewGuid(), Route = new RouteRequestData(Guid.NewGuid(), [], "x", ClimbingType.Sport, 0, null, null, null, null) };
        var (response, _) = await client.PUTAsync<UpdateRoute, UpdateRouteRequest, EmptyResponse>(updateReq);
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();

        var (_, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var created = routes.First();

        // Delete
        var deleteReq = new DeleteRouteRequest(created.Id);
        var (response, _) = await client.DELETEAsync<DeleteRoute, DeleteRouteRequest, EmptyResponse>(deleteReq);
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify
        var getReq = new GetRouteByIdRequest(created.Id);
        var (getResp, _) = await client.GETAsync<GetRouteById, GetRouteByIdRequest, RouteResponse>(getReq);
        getResp.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var deleteReq = new DeleteRouteRequest(Guid.NewGuid());
        var (response, _) = await client.DELETEAsync<DeleteRoute, DeleteRouteRequest, EmptyResponse>(deleteReq);
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_WithExistingPitches_UpdatesPitches()
    {
        using var client = fixture.CreateAuthenticatedClient();

        var (_, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var created = routes.First();

        // First, create a route with pitches
        var createRouteReq = new RouteRequestData(
            SectorId: sector.Id,
            Type: ClimbingType.Sport,
            Pitches: [
                new RoutePitchRequestData(
                    Id: null,
                    Name: "Original Pitch",
                    Type: ClimbingType.Sport,
                    Description: "Original description",
                    Grade: 500,
                    GearCount: 1,
                    Height: 10,
                    PitchOrder: 1
                )
            ],
            Name: "Route With Pitches",
            Description: "A route with pitches"
        );

        var (createResponse, routeWithPitches) = await client.POSTAsync<CreateRoute, RouteRequestData, RouteResponse>(createRouteReq);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Now update with existing pitch IDs (this tests the branch where p.Id != null)
        var updateRouteReq = new RouteRequestData(
            SectorId: sector.Id,
            Type: ClimbingType.Sport,
            Pitches: [
                new RoutePitchRequestData(
                    Id: routeWithPitches.Pitches?.First().Id,
                    Name: "Updated Pitch",
                    Type: ClimbingType.Sport,
                    Description: "Updated description",
                    Grade: 600,
                    GearCount: 2,
                    Height: 15,
                    PitchOrder: 1
                )
            ],
            Name: "Updated Route With Pitches",
            Description: "Updated route description"
        );

        var updateReq = new UpdateRouteRequest { RouteId = routeWithPitches.Id, Route = updateRouteReq };
        var (response, _) = await client.PUTAsync<UpdateRoute, UpdateRouteRequest, EmptyResponse>(updateReq);
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify the update
        var getReq = new GetRouteByIdRequest(routeWithPitches.Id);
        var (getResponse, updatedRoute) = await client.GETAsync<GetRouteById, GetRouteByIdRequest, RouteResponse>(getReq);
        getResponse.IsSuccessStatusCode.ShouldBeTrue();
        updatedRoute.Name.ShouldBe("Updated Route With Pitches");
        updatedRoute.Pitches.ShouldNotBeNull();
        updatedRoute.Pitches.Count().ShouldBe(1);
        updatedRoute.Pitches.First().Name.ShouldBe("Updated Pitch");
    }

    [Fact]
    public async Task Update_WithNoExistingPitches_CreatesNewPitch()
    {
        using var client = fixture.CreateAuthenticatedClient();

        var (_, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var created = routes.First(); // This route has no pitches initially

        // Update route without pitches (tests the branch where e.Pitches.Count == 0)
        var updateRouteReq = new RouteRequestData(
            SectorId: sector.Id,
            Type: ClimbingType.Sport,
            Pitches: [],
            Name: "Updated Route Without Pitches",
            Description: "Updated description",
            Grade: 500
        );

        var updateReq = new UpdateRouteRequest { RouteId = created.Id, Route = updateRouteReq };
        var (response, _) = await client.PUTAsync<UpdateRoute, UpdateRouteRequest, EmptyResponse>(updateReq);
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify the update - should create a default pitch when pitches list is empty
        var getReq = new GetRouteByIdRequest(created.Id);
        var (getResponse, updatedRoute) = await client.GETAsync<GetRouteById, GetRouteByIdRequest, RouteResponse>(getReq);
        getResponse.IsSuccessStatusCode.ShouldBeTrue();
        updatedRoute.Name.ShouldBe("Updated Route Without Pitches");
    }

    [Fact]
    public async Task Update_WithMixedPitchTypes_SetsRouteTypeToMixed()
    {
        using var client = fixture.CreateAuthenticatedClient();

        var (_, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var created = routes.First();

        // Update route with pitches of different types (tests the branch where types don't match)
        var updateRouteReq = new RouteRequestData(
            SectorId: sector.Id,
            Type: ClimbingType.Sport,
            Pitches: [
                new RoutePitchRequestData(
                    Id: null,
                    Name: "Sport Pitch",
                    Type: ClimbingType.Sport,
                    Description: "Sport pitch",
                    Grade: 500,
                    Height: 10,
                    GearCount: 1,
                    PitchOrder: 1
                ),
                new RoutePitchRequestData(
                    Id: null,
                    Name: "Trad Pitch",
                    Type: ClimbingType.Traditional,
                    Description: "Trad pitch",
                    Grade: 500,
                    Height: 10,
                    GearCount: 1,
                    PitchOrder: 2
                )
            ],
            Name: "Mixed Route",
            Description: "A route with mixed pitch types"
        );

        var updateReq = new UpdateRouteRequest { RouteId = created.Id, Route = updateRouteReq };
        var (response, _) = await client.PUTAsync<UpdateRoute, UpdateRouteRequest, EmptyResponse>(updateReq);
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify the route type is set to Mixed
        var getReq = new GetRouteByIdRequest(created.Id);
        var (getResponse, updatedRoute) = await client.GETAsync<GetRouteById, GetRouteByIdRequest, RouteResponse>(getReq);
        getResponse.IsSuccessStatusCode.ShouldBeTrue();
        updatedRoute.Type.ShouldBe(ClimbingType.Mixed);
    }
}


