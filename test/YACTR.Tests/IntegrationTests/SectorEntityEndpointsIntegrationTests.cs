using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Data.Model.Climbing;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using YACTR.Endpoints;
using YACTR.Endpoints.Sectors;
using YACTR.Endpoints.Areas;

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

        // Arrange - First create an area and sector
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var areaLocation = geometryFactory.CreatePoint(new Coordinate(-122.4194, 37.7749));
        var areaBoundary = geometryFactory.CreateMultiPolygon(new[] {
            geometryFactory.CreatePolygon(new[] {
                new Coordinate(-122.42, 37.77),
                new Coordinate(-122.42, 37.78),
                new Coordinate(-122.41, 37.78),
                new Coordinate(-122.41, 37.77),
                new Coordinate(-122.42, 37.77)
            })
        });

        var areaRequest = new AreaRequestData(
            "Test Area for GetById",
            "Test area description",
            areaLocation,
            areaBoundary
        );

        var (areaResponse, area) = await client.POSTAsync<CreateArea, AreaRequestData, Area>(areaRequest);
        areaResponse.IsSuccessStatusCode.ShouldBeTrue();

        var sectorArea = geometryFactory.CreatePolygon(new[] {
            new Coordinate(-122.419, 37.774),
            new Coordinate(-122.419, 37.775),
            new Coordinate(-122.418, 37.775),
            new Coordinate(-122.418, 37.774),
            new Coordinate(-122.419, 37.774)
        });

        var entryPoint = geometryFactory.CreatePoint(new Coordinate(-122.4185, 37.7745));

        var sectorRequest = new SectorRequestData(
            "Test Sector for GetById",
            sectorArea,
            entryPoint,
            null,
            null,
            area.Id
        );

        var (sectorResponse, createdSector) = await client.POSTAsync<CreateSector, SectorRequestData, Sector>(sectorRequest);
        sectorResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act
        var getRequest = new GetSectorByIdRequest(createdSector.Id);
        var (response, result) = await client.GETAsync<GetSectorById, GetSectorByIdRequest, Sector>(getRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdSector.Id);
        result.Name.ShouldBe("Test Sector for GetById");
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

        // Arrange - First create an area and sector
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var areaLocation = geometryFactory.CreatePoint(new Coordinate(-122.4194, 37.7749));
        var areaBoundary = geometryFactory.CreateMultiPolygon(new[] {
            geometryFactory.CreatePolygon(new[] {
                new Coordinate(-122.42, 37.77),
                new Coordinate(-122.42, 37.78),
                new Coordinate(-122.41, 37.78),
                new Coordinate(-122.41, 37.77),
                new Coordinate(-122.42, 37.77)
            })
        });

        var areaRequest = new AreaRequestData(
            "Test Area for Update",
            "Test area description",
            areaLocation,
            areaBoundary
        );

        var (areaResponse, area) = await client.POSTAsync<CreateArea, AreaRequestData, Area>(areaRequest);
        areaResponse.IsSuccessStatusCode.ShouldBeTrue();

        var sectorArea = geometryFactory.CreatePolygon(new[] {
            new Coordinate(-122.419, 37.774),
            new Coordinate(-122.419, 37.775),
            new Coordinate(-122.418, 37.775),
            new Coordinate(-122.418, 37.774),
            new Coordinate(-122.419, 37.774)
        });

        var entryPoint = geometryFactory.CreatePoint(new Coordinate(-122.4185, 37.7745));

        var sectorRequest = new SectorRequestData(
            "Test Sector for Update",
            sectorArea,
            entryPoint,
            null,
            null,
            area.Id
        );

        var (sectorResponse, createdSector) = await client.POSTAsync<CreateSector, SectorRequestData, Sector>(sectorRequest);
        sectorResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act
        var updateRequest = new UpdateSectorRequest()
        {
            SectorId = createdSector.Id,
            Data = new("", geometryFactory.CreatePolygon(), geometryFactory.CreatePoint(), null, null, area.Id)
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
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);


        // Act
        var updateRequest = new UpdateSectorRequest()
        {
            SectorId = invalidId,
            Data = new("", geometryFactory.CreatePolygon(), geometryFactory.CreatePoint(), null, null, Guid.NewGuid())
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

        // Arrange - First create an area and sector
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var areaLocation = geometryFactory.CreatePoint(new Coordinate(-122.4194, 37.7749));
        var areaBoundary = geometryFactory.CreateMultiPolygon(new[] {
            geometryFactory.CreatePolygon(new[] {
                new Coordinate(-122.42, 37.77),
                new Coordinate(-122.42, 37.78),
                new Coordinate(-122.41, 37.78),
                new Coordinate(-122.41, 37.77),
                new Coordinate(-122.42, 37.77)
            })
        });

        var areaRequest = new AreaRequestData(
            "Test Area for Delete",
            "Test area description",
            areaLocation,
            areaBoundary
        );

        var (areaResponse, area) = await client.POSTAsync<CreateArea, AreaRequestData, Area>(areaRequest);
        areaResponse.IsSuccessStatusCode.ShouldBeTrue();

        var sectorArea = geometryFactory.CreatePolygon(new[] {
            new Coordinate(-122.419, 37.774),
            new Coordinate(-122.419, 37.775),
            new Coordinate(-122.418, 37.775),
            new Coordinate(-122.418, 37.774),
            new Coordinate(-122.419, 37.774)
        });

        var entryPoint = geometryFactory.CreatePoint(new Coordinate(-122.4185, 37.7745));

        var sectorRequest = new SectorRequestData(
            "Test Sector for Delete",
            sectorArea,
            entryPoint,
            null,
            null,
            area.Id
        );

        var (sectorResponse, createdSector) = await client.POSTAsync<CreateSector, SectorRequestData, Sector>(sectorRequest);
        sectorResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act
        var deleteRequest = new DeleteSectorRequest(createdSector.Id);
        var (response, _) = await client.DELETEAsync<DeleteSector, DeleteSectorRequest, EmptyResponse>(deleteRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify the sector is actually deleted
        var getRequest = new GetSectorByIdRequest(createdSector.Id);
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