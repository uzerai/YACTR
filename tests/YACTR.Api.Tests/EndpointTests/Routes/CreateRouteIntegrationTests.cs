using System.Net;

using FastEndpoints.Testing;

using Shouldly;

using YACTR.Api.Endpoints.Routes;
using YACTR.Api.Tests.TestData;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Tests.EndpointTests.Routes;

[Collection("IntegrationTests")]
public class CreateRouteIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
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
            Pitches:
            [
                new RoutePitchRequestData(
                    Id: null,
                    Name: "Test Pitch",
                    Type: ClimbingType.Sport,
                    Description: "A sample pitch",
                    Grade: 500,
                    GearCount: 1,
                    Height: 10,
                    PitchOrder: 1)
            ],
            Name: "Test Route Create",
            Description: "A sample route",
            Grade: 500,
            FirstAscentClimberName: null,
            BolterName: null,
            TopoImageId: jpegImage.Id,
            TopoImageOverlayId: svgOverlayImage.Id);

        var (response, created) = await client.POSTAsync<CreateRoute, RouteRequestData, RouteResponse>(routeReq);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        created.ShouldNotBeNull();
        created.Id.ShouldNotBe(Guid.Empty);
        created.Name.ShouldBe("Test Route Create");
        created.TopoImageUrl.ShouldNotBeNull();
        created.TopoImageOverlayUrl.ShouldNotBeNull();
    }
}
