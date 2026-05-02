using System.Net;

using FastEndpoints.Testing;

using Shouldly;

using YACTR.Api.Endpoints.Pitches;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Tests.EndpointTests.Pitches;

[Collection("IntegrationTests")]
public class DeletePitchIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange - First create an area, sector, and pitch
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();

        var createRequest = new PitchRequestData(
            sector.Id,
            route.Id,
            "Test Pitch for Delete",
            ClimbingType.Sport,
            "Test description",
            "5.7",
            0
        );

        var (createResponse, createdPitch) = await client.POSTAsync<CreatePitch, PitchRequestData, Pitch>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act
        var deleteRequest = new DeletePitchRequest(createdPitch.Id);
        var (response, _) = await client.DELETEAsync<DeletePitch, DeletePitchRequest, EmptyResponse>(deleteRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify the pitch is actually deleted
        var getRequest = new GetPitchByIdRequest(createdPitch.Id);
        var (getResponse, _) = await client.GETAsync<GetPitchById, GetPitchByIdRequest, Pitch>(getRequest);
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var invalidId = Guid.NewGuid();

        // Act
        var deleteRequest = new DeletePitchRequest(invalidId);
        var (response, _) = await client.DELETEAsync<DeletePitch, DeletePitchRequest, EmptyResponse>(deleteRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
