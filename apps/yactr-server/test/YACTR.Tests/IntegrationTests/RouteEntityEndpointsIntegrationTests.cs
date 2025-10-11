using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Data.Model.Climbing;
using YACTR.Endpoints.Routes;
using YACTR.Tests.TestData;

namespace YACTR.Tests.Endpoints;

[Collection("IntegrationTests")]
public class RouteEntityEndpointsIntegrationTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
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
                    Name: "Test Pitch",
                    Type: ClimbingType.Sport,
                    Description: "A sample pitch",
                    Grade: "5.10a"
                )
            ],
            Name: "Test Route Create",
            Description: "A sample route",
            Grade: "5.10a",
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
            Grade: "",
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
        var updateReq = new UpdateRouteRequest { RouteId = Guid.NewGuid(), Route = new RouteRequestData(Guid.NewGuid(), [], "x", ClimbingType.Sport, null, null, null, null) };
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
}


