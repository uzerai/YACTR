using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Pitches;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Tests.EndpointTests.Pitches;

[Collection("IntegrationTests")]
public class UpdatePitchIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange - First create an area, sector, and pitch
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();

        var createRequest = new CreatePitchRequest(
            sector.Id,
            route.Id,
            "Test Pitch for Update",
            ClimbingType.Sport,
            "Original description",
            "5.8",
            0
        );

        var (createResponse, createdPitch) = await client.POSTAsync<CreatePitch, CreatePitchRequest, CreatePitchResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        var updateRequestData = new UpdatePitchData(
            sector.Id,
            route.Id,
            "Updated Pitch Name",
            ClimbingType.Sport,
            "Updated description",
            "5.8",
            0
        );

        // Act
        var updateRequest = new UpdatePitchRequest
        {
            PitchId = createdPitch.Id,
            Pitch = updateRequestData
        };
        var (response, _) = await client.PUTAsync<UpdatePitch, UpdatePitchRequest, EmptyResponse>(updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var invalidId = Guid.NewGuid();

        var updateRequestData = new UpdatePitchData(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Updated Pitch Name",
            ClimbingType.Sport,
            "Updated description",
            "5.8",
            0
        );

        // Act
        var updateRequest = new UpdatePitchRequest
        {
            PitchId = invalidId,
            Pitch = updateRequestData
        };
        var (response, _) = await client.PUTAsync<UpdatePitch, UpdatePitchRequest, EmptyResponse>(updateRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
