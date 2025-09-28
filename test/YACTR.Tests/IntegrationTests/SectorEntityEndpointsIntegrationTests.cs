using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Data.Model.Climbing;
using YACTR.Endpoints.Sectors;

namespace YACTR.Tests.Endpoints;

[Collection("IntegrationTests")]
public class SectorEntityEndpointsIntegrationTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
{
    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Act
        var (response, result) = await client.GETAsync<GetAllSectors, EmptyRequest, List<Sector>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsSector()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange
        var (area, sector, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        // Act
        var getRequest = new GetSectorByIdRequest(sector.Id);
        var (response, result) = await client.GETAsync<GetSectorById, GetSectorByIdRequest, Sector>(getRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(sector.Id);
        result.Name.ShouldBe(sector.Name);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var invalidId = Guid.NewGuid();

        // Act
        var getRequest = new GetSectorByIdRequest(invalidId);
        var (response, _) = await client.GETAsync<GetSectorById, GetSectorByIdRequest, Sector>(getRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange
        var (area, sector, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        // Act
        var updateRequest = new UpdateSectorRequest()
        {
            SectorId = sector.Id,
            Data = new("Updated Sector",
            fixture.TestDataSeeder.NewPolygon(),
            fixture.TestDataSeeder.NewPoint(),
            fixture.TestDataSeeder.NewPoint(),
            fixture.TestDataSeeder.NewLineString(),
            area.Id)
        };
        var (response, _) = await client.PUTAsync<UpdateSector, UpdateSectorRequest, EmptyResponse>(updateRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var invalidId = Guid.NewGuid();


        // Act
        var updateRequest = new UpdateSectorRequest()
        {
            SectorId = invalidId,
            Data = new("",
            fixture.TestDataSeeder.NewPolygon(),
            fixture.TestDataSeeder.NewPoint(),
            fixture.TestDataSeeder.NewPoint(),
            fixture.TestDataSeeder.NewLineString(),
            Guid.NewGuid())
        };
        var (response, _) = await client.PUTAsync<UpdateSector, UpdateSectorRequest, EmptyResponse>(updateRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange
        var (area, sector, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        // Act
        var deleteRequest = new DeleteSectorRequest(sector.Id);
        var (response, _) = await client.DELETEAsync<DeleteSector, DeleteSectorRequest, EmptyResponse>(deleteRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var getRequest = new GetSectorByIdRequest(sector.Id);
        var (getResponse, _) = await client.GETAsync<GetSectorById, GetSectorByIdRequest, Sector>(getRequest);
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var invalidId = Guid.NewGuid();

        // Act
        var deleteRequest = new DeleteSectorRequest(invalidId);
        var (response, _) = await client.DELETEAsync<DeleteSector, DeleteSectorRequest, EmptyResponse>(deleteRequest);

        // Assert  
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}