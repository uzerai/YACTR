using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Data.Model.Climbing;
using YACTR.Endpoints.Areas;
using YACTR.Endpoints.Routes;
using YACTR.Endpoints.Sectors;

namespace YACTR.Tests.Endpoints;

[Collection("IntegrationTests")]
public class RouteEntityEndpointsIntegrationTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
{
    private static async Task<(Area area, Sector sector)> CreateAreaAndSectorAsync(HttpClient client)
    {
        // Create area
        var areaReq = TestDataFactory.CreateAreaRequest("Routes Test Area");
        var (areaResponse, area) = await client.POSTAsync<CreateArea, AreaRequestData, Area>(areaReq);
        areaResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Create sector within area
        var sectorReq = TestDataFactory.CreateSectorRequest(area.Id, "Routes Test Sector");
        var (sectorResponse, sector) = await client.POSTAsync<CreateSector, SectorRequestData, Sector>(sectorReq);
        sectorResponse.IsSuccessStatusCode.ShouldBeTrue();

        return (area, sector);
    }

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

        var (_, sector) = await CreateAreaAndSectorAsync(client);

        var routeReq = new RouteRequestData(
            SectorId: sector.Id,
            Type: ClimbingType.Sport,
            Pitches: [],
            Name: "Test Route Create",
            Description: "A sample route",
            Grade: "5.10a",
            FirstAscentClimberName: null,
            BolterName: null
        );

        var (response, created) = await client.POSTAsync<CreateRoute, RouteRequestData, Route>(routeReq);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        created.ShouldNotBeNull();
        created.Id.ShouldNotBe(Guid.Empty);
        created.Name.ShouldBe("Test Route Create");
        created.SectorId.ShouldBe(sector.Id);
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsRoute()
    {
        using var client = fixture.CreateAuthenticatedClient();

        var (_, sector) = await CreateAreaAndSectorAsync(client);

        // Create a route first
        var routeReq = new RouteRequestData(
            SectorId: sector.Id,
            Type: ClimbingType.Sport,
            Pitches: [],
            Name: "Test Route Read",
            Description: null,
            Grade: null,
            FirstAscentClimberName: null,
            BolterName: null
        );
        var (createResp, created) = await client.POSTAsync<CreateRoute, RouteRequestData, Route>(routeReq);
        createResp.IsSuccessStatusCode.ShouldBeTrue();

        // Fetch it
        var getReq = new GetRouteByIdRequest(created.Id);
        var (response, result) = await client.GETAsync<GetRouteById, GetRouteByIdRequest, Route>(getReq);

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Name.ShouldBe("Test Route Read");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var getReq = new GetRouteByIdRequest(Guid.NewGuid());
        var (response, _) = await client.GETAsync<GetRouteById, GetRouteByIdRequest, Route>(getReq);
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_WithValidId_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();

        var (_, sector) = await CreateAreaAndSectorAsync(client);

        // Create route to update
        var routeReq = new RouteRequestData(
            SectorId: sector.Id,
            Type: ClimbingType.Sport,
            Pitches: [],
            Name: "Test Route Update",
            Description: null,
            Grade: null,
            FirstAscentClimberName: null,
            BolterName: null
        );
        var (createResp, created) = await client.POSTAsync<CreateRoute, RouteRequestData, Route>(routeReq);
        createResp.IsSuccessStatusCode.ShouldBeTrue();

        var updateReq = new UpdateRouteRequest { RouteId = created.Id, Route = routeReq };
        var (response, _) = await client.PUTAsync<UpdateRoute, UpdateRouteRequest, EmptyResponse>(updateReq);
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var updateReq = new UpdateRouteRequest { RouteId = Guid.NewGuid(), Route = new RouteRequestData(Guid.NewGuid(), [], "x", ClimbingType.Sport, null, null, null, null) };
        var (response, _) = await client.PUTAsync<UpdateRoute, UpdateRouteRequest, EmptyResponse>(updateReq);
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();

        var (_, sector) = await CreateAreaAndSectorAsync(client);

        // Create route to delete
        var routeReq = new RouteRequestData(
            SectorId: sector.Id,
            Type: ClimbingType.Sport,
            Pitches: [],
            Name: "Test Route Delete",
            Description: null,
            Grade: null,
            FirstAscentClimberName: null,
            BolterName: null
        );
        var (createResp, created) = await client.POSTAsync<CreateRoute, RouteRequestData, Route>(routeReq);
        createResp.IsSuccessStatusCode.ShouldBeTrue();

        // Delete
        var deleteReq = new DeleteRouteRequest(created.Id);
        var (response, _) = await client.DELETEAsync<DeleteRoute, DeleteRouteRequest, EmptyResponse>(deleteReq);
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify
        var getReq = new GetRouteByIdRequest(created.Id);
        var (getResp, _) = await client.GETAsync<GetRouteById, GetRouteByIdRequest, Route>(getReq);
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
}

internal static class TestDataFactory
{
    public static AreaRequestData CreateAreaRequest(string name)
    {
        var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

        var location = geometryFactory.CreatePoint(new NetTopologySuite.Geometries.Coordinate(-122.4194, 37.7749));
        var boundary = geometryFactory.CreateMultiPolygon(new[]
        {
            geometryFactory.CreatePolygon(new[]
            {
                new NetTopologySuite.Geometries.Coordinate(-122.42, 37.77),
                new NetTopologySuite.Geometries.Coordinate(-122.42, 37.78),
                new NetTopologySuite.Geometries.Coordinate(-122.41, 37.78),
                new NetTopologySuite.Geometries.Coordinate(-122.41, 37.77),
                new NetTopologySuite.Geometries.Coordinate(-122.42, 37.77)
            })
        });

        return new AreaRequestData(
            Name: name,
            Description: "Test area for routes",
            Location: location,
            Boundary: boundary
        );
    }

    public static SectorRequestData CreateSectorRequest(Guid areaId, string name)
    {
        var geometryFactory = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

        var sectorArea = geometryFactory.CreatePolygon(new[]
        {
            new NetTopologySuite.Geometries.Coordinate(-122.419, 37.774),
            new NetTopologySuite.Geometries.Coordinate(-122.419, 37.775),
            new NetTopologySuite.Geometries.Coordinate(-122.418, 37.775),
            new NetTopologySuite.Geometries.Coordinate(-122.418, 37.774),
            new NetTopologySuite.Geometries.Coordinate(-122.419, 37.774)
        });

        var entryPoint = geometryFactory.CreatePoint(new NetTopologySuite.Geometries.Coordinate(-122.4185, 37.7745));

        return new SectorRequestData(
            Name: name,
            SectorArea: sectorArea,
            EntryPoint: entryPoint,
            RecommendedParkingLocation: null,
            ApproachPath: null,
            AreaId: areaId
        );
    }
}


