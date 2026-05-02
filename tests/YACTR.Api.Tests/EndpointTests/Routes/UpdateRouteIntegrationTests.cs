using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Routes;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Tests.EndpointTests.Routes;

[Collection("IntegrationTests")]
public class UpdateRouteIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task Update_WithValidId_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (_, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var created = routes.First();

        var routeReq = new UpdateRouteRequestData(
            SectorId: sector.Id,
            Type: ClimbingType.Sport,
            Pitches: [],
            Name: "Test Route Update",
            Description: "Updated Test Route Description",
            Grade: 0,
            FirstAscentClimberName: "Updated Test Route First Ascent Climber Name",
            BolterName: "Updated Test Route Bolter Name");

        var (response, _) = await client.PUTAsync<UpdateRoute, UpdateRouteRequest, EmptyResponse>(new() { RouteId = created.Id, Route = routeReq });
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var updateReq = new UpdateRouteRequest
        {
            RouteId = Guid.NewGuid(),
            Route = new UpdateRouteRequestData(SectorId: Guid.NewGuid(), Pitches: [], Name: "x", Type: ClimbingType.Sport)
        };

        var (response, _) = await client.PUTAsync<UpdateRoute, UpdateRouteRequest, EmptyResponse>(updateReq);
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_WithExistingPitches_UpdatesPitches()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (_, sector, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var createRouteReq = new CreateRouteRequest(
            SectorId: sector.Id,
            Type: ClimbingType.Sport,
            Pitches:
            [
                new CreateRoutePitchRequest("Original Pitch", ClimbingType.Sport, null, 1, "Original description", 500, 10, 1)
            ],
            Name: "Route With Pitches",
            Description: "A route with pitches");

        var (createResponse, routeWithPitches) = await client.POSTAsync<CreateRoute, CreateRouteRequest, CreateRouteResponse>(createRouteReq);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        var updateRouteReq = new UpdateRouteRequestData(
            SectorId: sector.Id,
            Type: ClimbingType.Sport,
            Pitches:
            [
                new UpdateRoutePitchRequestData("Updated Pitch", ClimbingType.Sport, routeWithPitches.Pitches?.First().Id, 2, "Updated description", 600, 15, 1)
            ],
            Name: "Updated Route With Pitches",
            Description: "Updated route description");

        var (response, _) = await client.PUTAsync<UpdateRoute, UpdateRouteRequest, EmptyResponse>(new() { RouteId = routeWithPitches.Id, Route = updateRouteReq });
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var (getResponse, updatedRoute) = await client.GETAsync<GetRouteById, GetRouteByIdRequest, GetRouteByIdResponse>(new(routeWithPitches.Id));
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
        var created = routes.First();

        var updateRouteReq = new UpdateRouteRequestData(
            SectorId: sector.Id,
            Type: ClimbingType.Sport,
            Pitches: [],
            Name: "Updated Route Without Pitches",
            Description: "Updated description",
            Grade: 500);

        var (response, _) = await client.PUTAsync<UpdateRoute, UpdateRouteRequest, EmptyResponse>(new() { RouteId = created.Id, Route = updateRouteReq });
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var (getResponse, updatedRoute) = await client.GETAsync<GetRouteById, GetRouteByIdRequest, GetRouteByIdResponse>(new(created.Id));
        getResponse.IsSuccessStatusCode.ShouldBeTrue();
        updatedRoute.Name.ShouldBe("Updated Route Without Pitches");
    }

    [Fact]
    public async Task Update_WithMixedPitchTypes_SetsRouteTypeToMixed()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (_, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var created = routes.First();

        var updateRouteReq = new UpdateRouteRequestData(
            SectorId: sector.Id,
            Type: ClimbingType.Sport,
            Pitches:
            [
                new UpdateRoutePitchRequestData("Sport Pitch", ClimbingType.Sport, null, 1, "Sport pitch", 500, 10, 1),
                new UpdateRoutePitchRequestData("Trad Pitch", ClimbingType.Traditional, null, 1, "Trad pitch", 500, 10, 2)
            ],
            Name: "Mixed Route",
            Description: "A route with mixed pitch types");

        var (response, _) = await client.PUTAsync<UpdateRoute, UpdateRouteRequest, EmptyResponse>(new() { RouteId = created.Id, Route = updateRouteReq });
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var (getResponse, updatedRoute) = await client.GETAsync<GetRouteById, GetRouteByIdRequest, GetRouteByIdResponse>(new(created.Id));
        getResponse.IsSuccessStatusCode.ShouldBeTrue();
        updatedRoute.Type.ShouldBe(ClimbingType.Mixed);
    }
}
