using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Sectors;
using YACTR.Api.Pagination;

namespace YACTR.Api.Tests.EndpointTests;

[Collection("IntegrationTests")]
public class SectorEntityEndpointsIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Act
        var (response, result) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<SectorResponse>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsRequestedPageAndTotalCount()
    {
        using var client = fixture.CreateAuthenticatedClient();

        var (baselineResponse, baselineResult) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<SectorResponse>>(new()
        {
            Page = 1,
            PageSize = 1
        });
        baselineResponse.IsSuccessStatusCode.ShouldBeTrue();
        baselineResult.ShouldNotBeNull();

        for (var i = 0; i < 3; i++)
        {
            await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        }

        var (response, result) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<SectorResponse>>(new()
        {
            Page = 2,
            PageSize = 2
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(baselineResult.TotalCount + 3);
        var expectedPageCount = Math.Clamp(result.TotalCount - 2, 0, 2);
        result.Items.Count.ShouldBe(expectedPageCount);
    }

    [Fact]
    public async Task GetAll_WithPageSizeBelowMinimum_ClampsToMinimum()
    {
        using var client = fixture.CreateAuthenticatedClient();
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var (baselineResponse, baselineResult) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<SectorResponse>>(new()
        {
            Page = 1,
            PageSize = 1
        });
        baselineResponse.IsSuccessStatusCode.ShouldBeTrue();
        baselineResult.ShouldNotBeNull();

        var (response, result) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<SectorResponse>>(new()
        {
            Page = 1,
            PageSize = 0
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(baselineResult.TotalCount);
        result.Items.Count.ShouldBe(1);
        result.Items.Single().Id.ShouldBe(baselineResult.Items.Single().Id);
    }

    [Fact]
    public async Task GetAll_WithDifferentPages_ReturnsDifferentItems()
    {
        using var client = fixture.CreateAuthenticatedClient();
        for (var i = 0; i < 2; i++)
        {
            await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        }

        var (pageOneResponse, pageOneResult) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<SectorResponse>>(new()
        {
            Page = 1,
            PageSize = 1
        });

        var (pageTwoResponse, pageTwoResult) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<SectorResponse>>(new()
        {
            Page = 2,
            PageSize = 1
        });

        pageOneResponse.IsSuccessStatusCode.ShouldBeTrue();
        pageTwoResponse.IsSuccessStatusCode.ShouldBeTrue();
        pageOneResult.ShouldNotBeNull();
        pageTwoResult.ShouldNotBeNull();
        pageOneResult.TotalCount.ShouldBe(pageTwoResult.TotalCount);
        pageOneResult.Items.Count.ShouldBe(1);
        pageTwoResult.Items.Count.ShouldBe(1);
        pageOneResult.Items.Single().Id.ShouldNotBe(pageTwoResult.Items.Single().Id);
    }

    [Fact]
    public async Task CreateSector_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange
        var (area, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var createRequest = new SectorRequestData(
            "Test Sector",
            fixture.TestDataFactory.NewPolygon(),
            fixture.TestDataFactory.NewPoint(),
            area.Id,
            fixture.TestDataFactory.NewPoint(),
            fixture.TestDataFactory.NewLineString(),
            null,
            null
        );

        // Act
        var (response, result) = await client.POSTAsync<CreateSector, SectorRequestData, CreatedSectorResponse>(createRequest);

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
        var (response, result) = await client.GETAsync<GetSectorById, GetSectorByIdRequest, SectorResponse>(getRequest);

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
        var (response, _) = await client.GETAsync<GetSectorById, GetSectorByIdRequest, SectorResponse>(getRequest);

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
                fixture.TestDataFactory.NewPolygon(),
                fixture.TestDataFactory.NewPoint(),
                area.Id,
                fixture.TestDataFactory.NewPoint(),
                fixture.TestDataFactory.NewLineString(),
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
                fixture.TestDataFactory.NewPolygon(),
                fixture.TestDataFactory.NewPoint(),
                Guid.NewGuid(),
                fixture.TestDataFactory.NewPoint(),
                fixture.TestDataFactory.NewLineString(),
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
        var (getResponse, _) = await client.GETAsync<GetSectorById, GetSectorByIdRequest, SectorResponse>(getRequest);
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
            fixture.TestDataFactory.NewPolygon(),
            fixture.TestDataFactory.NewPoint(),
            area.Id,
            fixture.TestDataFactory.NewPoint(),
            fixture.TestDataFactory.NewLineString(),
            new[] { 
                new SectorImageRequestData(image1.Id, 1),
                new SectorImageRequestData(image2.Id, 2)
            },
            image1.Id // Primary image
        );

        // Act
        var (response, result) = await client.POSTAsync<CreateSector, SectorRequestData, CreatedSectorResponse>(createRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.SectorImageIds.Count().ShouldBe(2);
        result.SectorImageIds.ShouldContain(image1.Id);
        result.SectorImageIds.ShouldContain(image2.Id);
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
            fixture.TestDataFactory.NewPolygon(),
            fixture.TestDataFactory.NewPoint(),
            area.Id,
            fixture.TestDataFactory.NewPoint(),
            fixture.TestDataFactory.NewLineString(),
            new[] { 
                new SectorImageRequestData(image1.Id, 1),
                new SectorImageRequestData(image2.Id, 2)
            },
            null // No primary image specified
        );

        // Act
        var (response, result) = await client.POSTAsync<CreateSector, SectorRequestData, CreatedSectorResponse>(createRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.SectorImageIds.Count().ShouldBe(2);
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
            fixture.TestDataFactory.NewPolygon(),
            fixture.TestDataFactory.NewPoint(),
            area.Id,
            fixture.TestDataFactory.NewPoint(),
            fixture.TestDataFactory.NewLineString(),
            new[] { 
                new SectorImageRequestData(image1.Id, 1),
                new SectorImageRequestData(image2.Id, 2)
            },
            image1.Id
        );

        var (createResponse, createdSector) = await client.POSTAsync<CreateSector, SectorRequestData, CreatedSectorResponse>(createRequest);
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

    [Fact]
    public async Task UpdateSector_WithSectorImages_UpdatesOrdersWithoutDuplicatePivotRows()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange - create images first
        var image1 = await fixture.TestDataSeeder.CreateImageAsync();
        var image2 = await fixture.TestDataSeeder.CreateImageAsync();

        var (area, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var createRequest = new SectorRequestData(
            "Test Sector With Images For Update",
            fixture.TestDataFactory.NewPolygon(),
            fixture.TestDataFactory.NewPoint(),
            area.Id,
            fixture.TestDataFactory.NewPoint(),
            fixture.TestDataFactory.NewLineString(),
            new[]
            {
                new SectorImageRequestData(image1.Id, 1),
                new SectorImageRequestData(image2.Id, 2)
            },
            image1.Id // Primary image
        );

        var (createResponse, createdSector) = await client.POSTAsync<CreateSector, SectorRequestData, CreatedSectorResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act - update with the same image IDs but flipped order
        var updateRequest = new UpdateSectorRequest()
        {
            SectorId = createdSector.Id,
            Data = new SectorRequestData(
                "Updated Sector With Images For Update",
                fixture.TestDataFactory.NewPolygon(),
                fixture.TestDataFactory.NewPoint(),
                area.Id,
                fixture.TestDataFactory.NewPoint(),
                fixture.TestDataFactory.NewLineString(),
                new[]
                {
                    new SectorImageRequestData(image1.Id, 2),
                    new SectorImageRequestData(image2.Id, 1)
                },
                image2.Id // also flip primary
            )
        };

        var (updateResponse, _) = await client.PUTAsync<UpdateSector, UpdateSectorRequest, EmptyResponse>(updateRequest);
        updateResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Assert - ensure we updated existing pivot rows (no duplicates) and orders match
        var getRequest = new GetSectorByIdRequest(createdSector.Id);
        var (getResponse, updatedSector) = await client.GETAsync<GetSectorById, GetSectorByIdRequest, SectorResponse>(getRequest);

        getResponse.IsSuccessStatusCode.ShouldBeTrue();
        updatedSector.SectorImages.Count().ShouldBe(2);
        updatedSector.SectorImages.Single(si => si.ImageId == image1.Id).Order.ShouldBe(2);
        updatedSector.SectorImages.Single(si => si.ImageId == image2.Id).Order.ShouldBe(1);
        updatedSector.PrimarySectorImageId.ShouldBe(image2.Id);
    }
}