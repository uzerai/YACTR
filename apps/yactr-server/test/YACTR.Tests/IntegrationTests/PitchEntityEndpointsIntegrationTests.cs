using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Data.Model.Climbing;
using YACTR.Endpoints.Pitches;

namespace YACTR.Tests.Endpoints;

[Collection("IntegrationTests")]
public class PitchEntityEndpointsIntegrationTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
{
    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Act
        var (response, result) = await client.GETAsync<GetAllPitches, EmptyRequest, List<Pitch>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }

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
            "5.10a"
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
                "5.8"
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

    [Fact]
    public async Task GetById_WithValidId_ReturnsPitch()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange - First create an area, sector, and pitch
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();

        var createRequest = new PitchRequestData(
            sector.Id,
            route.Id,
            "Test Pitch for GetById",
            ClimbingType.Sport,
            "Test description",
            "5.9"
        );

        var (createResponse, createdPitch) = await client.POSTAsync<CreatePitch, PitchRequestData, Pitch>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act
        var getRequest = new GetPitchByIdRequest(createdPitch.Id);
        var (response, result) = await client.GETAsync<GetPitchById, GetPitchByIdRequest, Pitch>(getRequest);

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
        var (response, _) = await client.GETAsync<GetPitchById, GetPitchByIdRequest, Pitch>(getRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange - First create an area, sector, and pitch
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();

        var createRequest = new PitchRequestData(
            sector.Id,
            route.Id,
            "Test Pitch for Update",
            ClimbingType.Sport,
            "Original description",
            "5.8"
        );

        var (createResponse, createdPitch) = await client.POSTAsync<CreatePitch, PitchRequestData, Pitch>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        var updateRequestData = new PitchRequestData(
            sector.Id,
            route.Id,
            "Updated Pitch Name",
            ClimbingType.Sport,
            "Updated description",
            "5.8"
        );

        // Act
        var updateRequest = new UpdatePitchRequest(createdPitch.Id, updateRequestData);
        var (response, _) = await client.PUTAsync<UpdatePitch, UpdatePitchRequest, EmptyResponse>(updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var invalidId = Guid.NewGuid();

        var updateRequestData = new PitchRequestData(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Updated Pitch Name",
            ClimbingType.Sport,
            "Updated description",
            "5.8"
        );

        // Act
        var updateRequest = new UpdatePitchRequest(invalidId, updateRequestData);
        var (response, _) = await client.PUTAsync<UpdatePitch, UpdatePitchRequest, EmptyResponse>(updateRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

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
            "5.7"
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