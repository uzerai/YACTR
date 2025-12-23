using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Data.Model.Climbing;
using YACTR.Endpoints.Sectors;

namespace YACTR.Tests.EndpointTests;

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
    public async Task CreateSector_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange
        var (area, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var createRequest = new SectorRequestData(
            "Test Sector",
            fixture.TestDataSeeder.NewPolygon(),
            fixture.TestDataSeeder.NewPoint(),
            area.Id,
            fixture.TestDataSeeder.NewPoint(),
            fixture.TestDataSeeder.NewLineString(),
            null,
            null
        );

        // Act
        var (response, result) = await client.POSTAsync<CreateSector, SectorRequestData, SectorResponse>(createRequest);

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
            Data = new(
                "Updated Sector",
                fixture.TestDataSeeder.NewPolygon(),
                fixture.TestDataSeeder.NewPoint(),
                area.Id,
                fixture.TestDataSeeder.NewPoint(),
                fixture.TestDataSeeder.NewLineString(),
                null,
                null)
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
            Data = new(
                "",
                fixture.TestDataSeeder.NewPolygon(),
                fixture.TestDataSeeder.NewPoint(),
                Guid.NewGuid(),
                fixture.TestDataSeeder.NewPoint(),
                fixture.TestDataSeeder.NewLineString(),
                null,
                null)
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

    [Fact]
    public async Task CreateSector_WithSectorImages_CreatesSectorWithImages()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange - Create images first
        var image1 = await fixture.TestDataSeeder.CreateImageAsync();
        var image2 = await fixture.TestDataSeeder.CreateImageAsync();

        var (area, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var createRequest = new SectorRequestData(
            "Test Sector With Images",
            fixture.TestDataSeeder.NewPolygon(),
            fixture.TestDataSeeder.NewPoint(),
            area.Id,
            fixture.TestDataSeeder.NewPoint(),
            fixture.TestDataSeeder.NewLineString(),
            new[] { 
                new SectorImageRequestData(image1.Id, 1),
                new SectorImageRequestData(image2.Id, 2)
            },
            image1.Id // Primary image
        );

        // Act
        var (response, result) = await client.POSTAsync<CreateSector, SectorRequestData, SectorResponse>(createRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.SectorImages.ShouldNotBeNull();
        result.SectorImages.Count().ShouldBe(2);
        result.PrimarySectorImageId.ShouldBe(image1.Id);
    }

    [Fact]
    public async Task CreateSector_WithSectorImagesButNoPrimary_UsesFirstImageAsPrimary()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange - Create images first
        var image1 = await fixture.TestDataSeeder.CreateImageAsync();
        var image2 = await fixture.TestDataSeeder.CreateImageAsync();

        var (area, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var createRequest = new SectorRequestData(
            "Test Sector With Images No Primary",
            fixture.TestDataSeeder.NewPolygon(),
            fixture.TestDataSeeder.NewPoint(),
            area.Id,
            fixture.TestDataSeeder.NewPoint(),
            fixture.TestDataSeeder.NewLineString(),
            new[] { 
                new SectorImageRequestData(image1.Id, 1),
                new SectorImageRequestData(image2.Id, 2)
            },
            null // No primary image specified
        );

        // Act
        var (response, result) = await client.POSTAsync<CreateSector, SectorRequestData, SectorResponse>(createRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.PrimarySectorImageId.ShouldBe(image1.Id); // Should use first image as primary
    }

    [Fact]
    public async Task GetSectorById_WithSectorImages_ReturnsSectorWithImageUrls()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange - Create a sector with images
        var image1 = await fixture.TestDataSeeder.CreateImageAsync();
        var image2 = await fixture.TestDataSeeder.CreateImageAsync();

        var (area, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var createRequest = new SectorRequestData(
            "Test Sector With Images For Get",
            fixture.TestDataSeeder.NewPolygon(),
            fixture.TestDataSeeder.NewPoint(),
            area.Id,
            fixture.TestDataSeeder.NewPoint(),
            fixture.TestDataSeeder.NewLineString(),
            new[] { 
                new SectorImageRequestData(image1.Id, 1),
                new SectorImageRequestData(image2.Id, 2)
            },
            image1.Id
        );

        var (createResponse, createdSector) = await client.POSTAsync<CreateSector, SectorRequestData, SectorResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act
        var getRequest = new GetSectorByIdRequest(createdSector.Id);
        var (response, result) = await client.GETAsync<GetSectorById, GetSectorByIdRequest, SectorResponse>(getRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.SectorImages.ShouldNotBeNull();
        result.SectorImages.Count().ShouldBe(2);
        // Verify that image URLs are populated (coverage for the async mapping branch)
        result.SectorImages.All(img => !string.IsNullOrEmpty(img.ImageUrl)).ShouldBeTrue();
    }
}