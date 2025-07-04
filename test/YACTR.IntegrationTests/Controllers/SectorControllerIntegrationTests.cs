using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using YACTR.DTO.RequestData;
using YACTR.Data.Model.Location;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using System.Net;
using System.Text.Json.Serialization;
using NetTopologySuite.IO.Converters;
using System.Reflection;

namespace YACTR.IntegrationTests.Controllers;

public class SectorControllerIntegrationTests : IntegrationTestClassFixture
{
    public SectorControllerIntegrationTests(TestWebApplicationFactory factory): base(factory)
    {
    }

    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        var client = CreateAuthenticatedClient();
        // Act
        var response = await client.GetAsync("/sectors");

        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task GetById_WithValidId_ReturnsSector()
    {
        var client = CreateAuthenticatedClient();
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
        
        var areaContent = new StringContent(
            JsonSerializer.Serialize(areaRequest, _jsonSerializerOptions),
            Encoding.UTF8,
            "application/json");
            
        var areaResponse = await client.PostAsync("/areas", areaContent);
        areaResponse.EnsureSuccessStatusCode();
        
        var area = JsonSerializer.Deserialize<Area>(
            await areaResponse.Content.ReadAsStringAsync(),
            _jsonSerializerOptions
        );
        
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
            area!.Id
        );
        
        var sectorContent = new StringContent(
            JsonSerializer.Serialize(sectorRequest, _jsonSerializerOptions),
            Encoding.UTF8,
            "application/json");
            
        var sectorResponse = await client.PostAsync("/sectors", sectorContent);
        sectorResponse.EnsureSuccessStatusCode();
        
        var createdSector = JsonSerializer.Deserialize<Sector>(
            await sectorResponse.Content.ReadAsStringAsync(),
            _jsonSerializerOptions
        );
        
        // Act
        var response = await client.GetAsync($"/sectors/{createdSector!.Id}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var sector = JsonSerializer.Deserialize<Sector>(
            await response.Content.ReadAsStringAsync(),
            _jsonSerializerOptions
        );
        Assert.NotNull(sector);
        Assert.Equal(createdSector.Id, sector.Id);
        Assert.Equal("Test Sector for GetById", sector.Name);
    }
    
    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();
        // Arrange
        var invalidId = Guid.NewGuid();
        
        // Act
        var response = await client.GetAsync($"/sectors/{invalidId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        var client = CreateAuthenticatedClient();
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
        
        var areaContent = new StringContent(
            JsonSerializer.Serialize(areaRequest, _jsonSerializerOptions),
            Encoding.UTF8,
            "application/json");
            
        var areaResponse = await client.PostAsync("/areas", areaContent);
        areaResponse.EnsureSuccessStatusCode();
        
        var area = JsonSerializer.Deserialize<Area>(
            await areaResponse.Content.ReadAsStringAsync(),
            _jsonSerializerOptions
        );
        
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
            area!.Id
        );
        
        var sectorContent = new StringContent(
            JsonSerializer.Serialize(sectorRequest, _jsonSerializerOptions),
            Encoding.UTF8,
            "application/json");
            
        var sectorResponse = await client.PostAsync("/sectors", sectorContent);
        sectorResponse.EnsureSuccessStatusCode();
        
        var createdSector = JsonSerializer.Deserialize<Sector>(
            await sectorResponse.Content.ReadAsStringAsync(),
            _jsonSerializerOptions
        );
        
        // Create update request
        var updatedSectorArea = geometryFactory.CreatePolygon(new[] {
            new Coordinate(-122.419, 37.774),
            new Coordinate(-122.419, 37.776),
            new Coordinate(-122.417, 37.776),
            new Coordinate(-122.417, 37.774),
            new Coordinate(-122.419, 37.774)
        });
        
        var updateRequest = new SectorRequestData(
            "Updated Sector Name",
            updatedSectorArea,
            entryPoint,
            null,
            null,
            area.Id
        );
        
        var updateContent = new StringContent(
            JsonSerializer.Serialize(updateRequest, _jsonSerializerOptions),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await client.PutAsync($"/sectors/{createdSector!.Id}", updateContent);
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();
        // Arrange
        var invalidId = Guid.NewGuid();
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var sectorArea = geometryFactory.CreatePolygon(new[] {
            new Coordinate(-122.419, 37.774),
            new Coordinate(-122.419, 37.775),
            new Coordinate(-122.418, 37.775),
            new Coordinate(-122.418, 37.774),
            new Coordinate(-122.419, 37.774)
        });
        
        var entryPoint = geometryFactory.CreatePoint(new Coordinate(-122.4185, 37.7745));
        
        var updateRequest = new SectorRequestData(
            "Updated Name",
            sectorArea,
            entryPoint,
            null,
            null,
            Guid.NewGuid()
        );
        
        var content = new StringContent(
            JsonSerializer.Serialize(updateRequest, _jsonSerializerOptions),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await client.PutAsync($"/sectors/{invalidId}", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        var client = CreateAuthenticatedClient();
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
        
        var areaContent = new StringContent(
            JsonSerializer.Serialize(areaRequest, _jsonSerializerOptions),
            Encoding.UTF8,
            "application/json");
            
        var areaResponse = await client.PostAsync("/areas", areaContent);
        areaResponse.EnsureSuccessStatusCode();
        
        var area = JsonSerializer.Deserialize<Area>(
            await areaResponse.Content.ReadAsStringAsync(),
            _jsonSerializerOptions
        );
        
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
            area!.Id
        );
        
        var sectorContent = new StringContent(
            JsonSerializer.Serialize(sectorRequest, _jsonSerializerOptions),
            Encoding.UTF8,
            "application/json");
            
        var sectorResponse = await client.PostAsync("/sectors", sectorContent);
        sectorResponse.EnsureSuccessStatusCode();
        
        var createdSector = JsonSerializer.Deserialize<Sector>(
            await sectorResponse.Content.ReadAsStringAsync(),
            _jsonSerializerOptions
        );
        
        // Act
        var response = await client.DeleteAsync($"/sectors/{createdSector!.Id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Verify the sector is actually deleted
        var getResponse = await client.GetAsync($"/sectors/{createdSector.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
    
    [Fact]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();
        // Arrange
        var invalidId = Guid.NewGuid();
        
        // Act
        var response = await client.DeleteAsync($"/sectors/{invalidId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
} 