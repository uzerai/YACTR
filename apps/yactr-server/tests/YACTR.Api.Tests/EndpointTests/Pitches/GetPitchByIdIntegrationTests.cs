using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Pitches;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Tests.EndpointTests.Pitches;

[Collection("IntegrationTests")]
public class GetPitchByIdIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task GetById_WithValidId_ReturnsPitch()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange - First create an area, sector, and pitch
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();

        var createRequest = new CreatePitchRequest(
            sector.Id,
            route.Id,
            "Test Pitch for GetById",
            ClimbingType.Sport,
            "Test description",
            "5.9",
            0
        );

        var (createResponse, createdPitch) = await client.POSTAsync<CreatePitch, CreatePitchRequest, CreatePitchResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act
        var getRequest = new GetPitchByIdRequest(createdPitch.Id);
        var (response, result) = await client.GETAsync<GetPitchById, GetPitchByIdRequest, GetPitchByIdResponse>(getRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdPitch.Id);
        result.Name.ShouldBe("Test Pitch for GetById");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var invalidId = Guid.NewGuid();

        // Act
        var getRequest = new GetPitchByIdRequest(invalidId);
        var (response, _) = await client.GETAsync<GetPitchById, GetPitchByIdRequest, GetPitchByIdResponse>(getRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
