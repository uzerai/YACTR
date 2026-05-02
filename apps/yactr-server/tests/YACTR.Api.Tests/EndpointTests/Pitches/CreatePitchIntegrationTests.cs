using System.Net;

using FastEndpoints.Testing;

using Shouldly;

using YACTR.Api.Endpoints.Pitches;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Tests.EndpointTests.Pitches;

[Collection("IntegrationTests")]
public class CreatePitchIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedPitch()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange - First create an area and sector
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();

        // Create pitch request
        var createRequest = new PitchRequestData(
            sector.Id,
            route.Id,
            "Test Pitch",
            ClimbingType.Sport,
            "A challenging sport pitch",
            "5.10a",
            0
        );

        // Act
        var (response, result) = await client.POSTAsync<CreatePitch, PitchRequestData, Pitch>(createRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Test Pitch");
        result.Type.ShouldBe(ClimbingType.Sport);
        result.Description.ShouldBe("A challenging sport pitch");
        result.SectorId.ShouldBe(sector.Id);
    }

    [Fact]
    public async Task Create_WithDifferentPitchTypes_ReturnsCreatedPitch()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange - First create an area and sector
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();

        // Test different pitch types
        var pitchTypes = new[] { ClimbingType.Traditional, ClimbingType.Mixed, ClimbingType.Aid };

        foreach (var pitchType in pitchTypes)
        {
            var createRequest = new PitchRequestData(
                sector.Id,
                route.Id,
                $"Test {pitchType} Pitch",
                pitchType,
                $"A {pitchType.ToString().ToLower()} pitch",
                "5.8",
                0
            );

            // Act
            var (response, result) = await client.POSTAsync<CreatePitch, PitchRequestData, Pitch>(createRequest);

            // Assert
            response.IsSuccessStatusCode.ShouldBeTrue();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            result.ShouldNotBeNull();
            result.Type.ShouldBe(pitchType);
        }
    }
}
